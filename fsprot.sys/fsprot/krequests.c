#include "krequests.h"

NTSTATUS AddPathForProtection(
    PVOID InputBuffer,
    DWORD InputBufferSize,
    PVOID OutputBuffer,
    DWORD OutputBufferSize )
{
    UNREFERENCED_PARAMETER( InputBuffer );
    UNREFERENCED_PARAMETER( InputBufferSize );
    UNREFERENCED_PARAMETER( OutputBuffer );
    UNREFERENCED_PARAMETER( OutputBufferSize );

    if (CheckIRQL( PASSIVE_LEVEL ) == FALSE)
    {
        DbgPrint( "AddPathForProtection() incorrect IRQL == %d", (int)KeGetCurrentIrql() );
        return STATUS_UNSUCCESSFUL;
    }

    if (InputBufferSize < sizeof(KADD_PATH_FOR_PROTECTION_IN_PACKET))
    {
        return STATUS_UNSUCCESSFUL;
    }

    if (OutputBufferSize < sizeof(KADD_PATH_FOR_PROTECTION_OUT_PACKET))
    {
        return STATUS_UNSUCCESSFUL;
    }

    KIRQL oldIrql;

    KeAcquireSpinLock( &(GetDeviceExt()->FileListAccessSpinLock), &oldIrql );

    PKADD_PATH_FOR_PROTECTION_IN_PACKET inPacket = InputBuffer;
    PKADD_PATH_FOR_PROTECTION_OUT_PACKET outPacket = OutputBuffer;

    PFILE_OBJECT_LIST_ENTRY entry = K_InsertLast( &(GetDeviceExt()->FileListHead) );

    wcscpy( entry->fullPath, inPacket->FullPath );
    entry->uniqueKey = GetDeviceExt()->LastUniqueKey;
    entry->weakProtection = inPacket->WeakProtection;

    outPacket->UniqueKey = entry->uniqueKey;

    GetDeviceExt()->LastUniqueKey++;

    KeReleaseSpinLock( &(GetDeviceExt()->FileListAccessSpinLock), oldIrql );

    return STATUS_SUCCESS;
}

NTSTATUS RemovePathFromProtection(
    PVOID InputBuffer,
    DWORD InputBufferSize,
    PVOID OutputBuffer,
    DWORD OutputBufferSize )
{
    UNREFERENCED_PARAMETER( InputBuffer );
    UNREFERENCED_PARAMETER( InputBufferSize );
    UNREFERENCED_PARAMETER( OutputBuffer );
    UNREFERENCED_PARAMETER( OutputBufferSize );

    if (CheckIRQL( PASSIVE_LEVEL ) == FALSE)
    {
        DbgPrint( "RemovePathFromProtection() incorrect IRQL == %d", (int)KeGetCurrentIrql() );
        return STATUS_UNSUCCESSFUL;
    }

    if (InputBufferSize < sizeof(KREMOVE_PATH_FROM_PROTECTION_IN_PACKET))
    {
        return STATUS_UNSUCCESSFUL;
    }

    if (OutputBufferSize < sizeof(KREMOVE_PATH_FROM_PROTECTION_OUT_PACKET))
    {
        return STATUS_UNSUCCESSFUL;
    }

    KIRQL oldIrql;

    KeAcquireSpinLock( &(GetDeviceExt()->FileListAccessSpinLock), &oldIrql );

    PKREMOVE_PATH_FROM_PROTECTION_IN_PACKET inPacket = InputBuffer;
    PKREMOVE_PATH_FROM_PROTECTION_OUT_PACKET outPacket = OutputBuffer;

    PFILE_OBJECT_LIST_ENTRY entry = GetDeviceExt()->FileListHead;

    while (entry != NULL)
    {
        if (entry->uniqueKey == inPacket->UniqueKey)
        {
            break;
        }

        entry = K_GetNextEntry( entry );
    }

    BOOLEAN result = K_RemoveEntry( &(GetDeviceExt()->FileListHead), entry );

    outPacket->RemoveResult = result;

    KeReleaseSpinLock( &(GetDeviceExt()->FileListAccessSpinLock), oldIrql );

    return STATUS_SUCCESS;
}

NTSTATUS RemoveAllPathsFromProtection(
    PVOID InputBuffer,
    DWORD InputBufferSize,
    PVOID OutputBuffer,
    DWORD OutputBufferSize )
{
    UNREFERENCED_PARAMETER( InputBuffer );
    UNREFERENCED_PARAMETER( InputBufferSize );
    UNREFERENCED_PARAMETER( OutputBuffer );
    UNREFERENCED_PARAMETER( OutputBufferSize );

    if (CheckIRQL( PASSIVE_LEVEL ) == FALSE)
    {
        DbgPrint( "RemoveAllPathsFromProtection() incorrect IRQL == %d", (int)KeGetCurrentIrql() );
        return STATUS_UNSUCCESSFUL;
    }

    KIRQL oldIrql;

    KeAcquireSpinLock( &(GetDeviceExt()->FileListAccessSpinLock), &oldIrql );

    K_ReleaseList( &(GetDeviceExt()->FileListHead) );

    KeReleaseSpinLock( &(GetDeviceExt()->FileListAccessSpinLock), oldIrql );

    return STATUS_SUCCESS;
}

NTSTATUS SetUnloadAccess(
    PVOID InputBuffer,
    DWORD InputBufferSize,
    PVOID OutputBuffer,
    DWORD OutputBufferSize )
{
    UNREFERENCED_PARAMETER( InputBuffer );
    UNREFERENCED_PARAMETER( InputBufferSize );
    UNREFERENCED_PARAMETER( OutputBuffer );
    UNREFERENCED_PARAMETER( OutputBufferSize );

    if (CheckIRQL( PASSIVE_LEVEL ) == FALSE)
    {
        DbgPrint( "SetUnloadAccess() incorrect IRQL == %d", (int)KeGetCurrentIrql() );
        return STATUS_UNSUCCESSFUL;
    }

    if (InputBufferSize < sizeof(KSET_UNLOAD_ACCESS_IN_PACKET))
    {
        return STATUS_UNSUCCESSFUL;
    }

    PKSET_UNLOAD_ACCESS_IN_PACKET inPacket = InputBuffer;

    SetDriverUnloadAccess( inPacket->CanUnload );

    return STATUS_SUCCESS;
}