#include "kirp_dispatchers.h"

BOOLEAN GetStackIO(
    PIRP Irp,
    PVOID *InputBuffer,
    DWORD *InputBufferSize,
    PVOID *OutputBuffer,
    DWORD *OutputBufferSize,
    DWORD *ControlCode )
{
    if (Irp == NULL)
    {
        return FALSE;
    }

    PIO_STACK_LOCATION stack = IoGetCurrentIrpStackLocation( Irp );

    if (stack != NULL)
    {
        *InputBuffer = Irp->UserBuffer;
        *InputBufferSize = stack->Parameters.DeviceIoControl.InputBufferLength;

        *OutputBuffer = Irp->UserBuffer;
        *OutputBufferSize = stack->Parameters.DeviceIoControl.OutputBufferLength;

        *ControlCode = stack->Parameters.DeviceIoControl.IoControlCode;
        return TRUE;
    }
    else
    {
        *InputBuffer = NULL;
        *OutputBuffer = NULL;
        *InputBufferSize = 0;
        *OutputBufferSize = 0;
        *ControlCode = 0;
        return FALSE;
    }
}

NTSTATUS IrpDefaultDispatch( IN PDEVICE_OBJECT DeviceObject,
                             IN PIRP Irp )
{
    /* IRQL == PASSIVE_LEVEL */

    UNREFERENCED_PARAMETER( DeviceObject );
    UNREFERENCED_PARAMETER( Irp );

    Irp->IoStatus.Information = 0;
    Irp->IoStatus.Status = STATUS_SUCCESS;
    IoCompleteRequest( Irp, IO_NO_INCREMENT );
    return Irp->IoStatus.Status;
}

NTSTATUS IrpDeviceControlDispatch( IN PDEVICE_OBJECT DeviceObject, IN PIRP Irp )
{
    UNREFERENCED_PARAMETER( DeviceObject );
    UNREFERENCED_PARAMETER( Irp );

    /* IRQL == PASSIVE_LEVEL */

    NTSTATUS status = STATUS_SUCCESS;

    PVOID inputBuffer, outputBuffer;
    DWORD inputBufferSize, outputBufferSize;
    DWORD controlCode;

    if (GetStackIO( Irp, &inputBuffer, &inputBufferSize, &outputBuffer, &outputBufferSize, &controlCode ))
    {
        switch (controlCode)
        {
            case ADD_PATH_FOR_PROTECTION:
                status = AddPathForProtection( inputBuffer, inputBufferSize, outputBuffer, outputBufferSize );
                break;

            case REMOVE_PATH_FROM_PROTECTION:
                status = RemovePathFromProtection( inputBuffer, inputBufferSize, outputBuffer, outputBufferSize );
                break;

            case REMOVE_ALL_PATHS_FROM_PROTECTION:
                status = RemoveAllPathsFromProtection( inputBuffer, inputBufferSize, outputBuffer, outputBufferSize );
                break;

            case SET_UNLOAD_ACCESS:
                status = SetUnloadAccess( inputBuffer, inputBufferSize, outputBuffer, outputBufferSize );
                break;

            default:
                DbgPrint( "unknown io control code" );
                break;
        }

    }
    else
    {
        DbgPrint( "GetStackIO() == FALSE" );
    }

    Irp->IoStatus.Information = 0;
    Irp->IoStatus.Status = status;
    IoCompleteRequest( Irp, IO_NO_INCREMENT );

    return Irp->IoStatus.Status;
}

