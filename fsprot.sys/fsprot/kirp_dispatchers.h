#ifndef _KIRP_DISPATCHERS_H
#define _KIRP_DISPATCHERS_H

#include <fltKernel.h>
#include <ntstrsafe.h>
#include <wdm.h>
#include "krequests.h"

#define ADD_PATH_FOR_PROTECTION CTL_CODE(FILE_DEVICE_UNKNOWN, 0x800, METHOD_BUFFERED, FILE_ANY_ACCESS )
#define REMOVE_PATH_FROM_PROTECTION CTL_CODE(FILE_DEVICE_UNKNOWN, 0x801, METHOD_BUFFERED, FILE_ANY_ACCESS )
#define REMOVE_ALL_PATHS_FROM_PROTECTION CTL_CODE(FILE_DEVICE_UNKNOWN, 0x802, METHOD_BUFFERED, FILE_ANY_ACCESS )
#define SET_UNLOAD_ACCESS CTL_CODE(FILE_DEVICE_UNKNOWN, 0x803, METHOD_BUFFERED, FILE_ANY_ACCESS )

BOOLEAN GetStackIO(
    PIRP Irp,
    PVOID *InputBuffer,
    DWORD *InputBufferSize,
    PVOID *OutputBuffer,
    DWORD *OutputBufferSize,
    DWORD *ControlCode );

NTSTATUS IrpDefaultDispatch( IN PDEVICE_OBJECT DeviceObject,
                             IN PIRP Irp );

NTSTATUS IrpDeviceControlDispatch( IN PDEVICE_OBJECT DeviceObject,
                                   IN PIRP Irp );

#endif

