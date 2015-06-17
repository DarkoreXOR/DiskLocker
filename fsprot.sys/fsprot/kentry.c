#include "kentry.h"

NTSTATUS DriverEntry( IN PDRIVER_OBJECT DriverObject,
                      IN PUNICODE_STRING RegistryPath )
{
    /* IRQL == PASSIVE_LEVEL */

    UNREFERENCED_PARAMETER( DriverObject );
    UNREFERENCED_PARAMETER( RegistryPath );

    NTSTATUS status;

    DbgPrint( "file system protection filter start" );

    /* initialize IRP */

    InitializeIrpDispatchers( DriverObject );

    /* create device */

    UNICODE_STRING deviceObjectName;
    RtlInitUnicodeString( &deviceObjectName, L"\\device\\disklocker_fsprotdrv" );

    PDEVICE_OBJECT DeviceObject;
    status = CreateIoDevice( deviceObjectName, DriverObject, &DeviceObject );

    if (!NT_SUCCESS( status ))
    {
        DbgPrint( "CreateIoDevice() failed with code = %08x\n", status );
        return status;
    }

    SetDeviceExt( DeviceObject->DeviceExtension );

    /* initialize device extension */

    GetDeviceExt()->DeviceObject = DeviceObject;
    GetDeviceExt()->DriverObject = DriverObject;
    GetDeviceExt()->UnloadRoutine = OnUnload;
    GetDeviceExt()->FileListHead = NULL;
    GetDeviceExt()->LastUniqueKey = 1;

    KeInitializeSpinLock( &(GetDeviceExt()->FileListAccessSpinLock) );

    /* create symlink */

    RtlInitUnicodeString( &SymbolicLinkName, L"\\DosDevices\\disklocker_fsprotdrv" );

    status = IoCreateSymbolicLink( &SymbolicLinkName, &deviceObjectName );

    if (!NT_SUCCESS( status ))
    {
        /* delete registered device */
        IoDeleteDevice( GetDeviceExt()->DeviceObject );

        DbgPrint( "IoCreateSymbolicLink() failed with code = %08x\n", status );
        return status;
    }

    /* register filter */

    status = FltRegisterFilter( DriverObject, &FilterRegistration, &(GetDeviceExt()->FltFilter) );

    if (!NT_SUCCESS( status ))
    {
        /* delete registered symbolic link */
        IoDeleteSymbolicLink( &SymbolicLinkName );

        /* delete registered device */
        IoDeleteDevice( GetDeviceExt()->DeviceObject );

        DbgPrint( "FltRegisterFilter() failed with code = %08x\n", status );
        return status;
    }

    /* start filtering */

    status = FltStartFiltering( GetDeviceExt()->FltFilter );
    if (!NT_SUCCESS( status ))
    {
        /* delete registered symbolic link */
        IoDeleteSymbolicLink( &SymbolicLinkName );

        /* delete registered device */
        IoDeleteDevice( GetDeviceExt()->DeviceObject );

        /* unregister filter */
        FltUnregisterFilter( GetDeviceExt()->FltFilter );

        DbgPrint( "FltStartFiltering() failed with code = %08x\n", status );
        return status;
    }

    /* initialization done */

    DbgPrint( "file system filter initialization done..." );
    return STATUS_SUCCESS;
}

NTSTATUS CreateIoDevice( IN UNICODE_STRING DeviceObjectName,
                         IN PDRIVER_OBJECT DriverObject,
                         OUT PDEVICE_OBJECT *DeviceObject )
{
    UNREFERENCED_PARAMETER( DriverObject );
    UNREFERENCED_PARAMETER( DeviceObject );

    NTSTATUS status = IoCreateDevice(
        DriverObject,
        sizeof(DEVICE_EXT_DATA),
        &DeviceObjectName,
        FILE_DEVICE_UNKNOWN,
        FILE_DEVICE_SECURE_OPEN,
        FALSE,
        DeviceObject );

    /* buffered I/O for DeviceIoControl() */
    SetFlag( (*DeviceObject)->Flags, DO_BUFFERED_IO );

    /* multiple CreateFile() */
    ClearFlag( (*DeviceObject)->Flags, DO_EXCLUSIVE );

    /* TODO: file system device flag */
    SetFlag( (*DeviceObject)->Characteristics, FILE_DEVICE_SECURE_OPEN );

    return status;
}


void InitializeIrpDispatchers( IN PDRIVER_OBJECT DriverObject )
{
    int i;

    for (i = 0; i < IRP_MJ_MAXIMUM_FUNCTION; i++)
    {
        DriverObject->MajorFunction[i] = IrpDefaultDispatch;
    }

    DriverObject->MajorFunction[IRP_MJ_DEVICE_CONTROL] = IrpDeviceControlDispatch;

    DriverObject->DriverUnload = OnUnload;
}

void OnUnload( IN PDRIVER_OBJECT DriverObject )
{
    /* IRQL == PASSIVE_LEVEL */

    UNREFERENCED_PARAMETER( DriverObject );

    DbgPrint( "file system driver unloading..." );

    KIRQL oldIrql;

    KeAcquireSpinLock( &(GetDeviceExt()->FileListAccessSpinLock), &oldIrql );

    K_ReleaseList( &(GetDeviceExt()->FileListHead) );

    KeReleaseSpinLock( &(GetDeviceExt()->FileListAccessSpinLock), oldIrql );

    /* unregister filter */
    FltUnregisterFilter( GetDeviceExt()->FltFilter );

    /* delete registered symbolic link */
    IoDeleteSymbolicLink( &SymbolicLinkName );

    /* delete registered device */
    IoDeleteDevice( GetDeviceExt()->DeviceObject );
}
