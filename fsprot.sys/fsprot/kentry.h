#ifndef _KENTRY_H
#define _KENTRY_H

#include <fltKernel.h>
#include <ntstrsafe.h>
#include <wdm.h>
#include "kdevice_ext.h"
#include "kregistration.h"
#include "kirp_dispatchers.h"

void OnUnload( IN PDRIVER_OBJECT DriverObject );

NTSTATUS CreateIoDevice( IN UNICODE_STRING DeviceObjectName,
                         IN PDRIVER_OBJECT DriverObject,
                         OUT PDEVICE_OBJECT *DeviceObject );

void InitializeIrpDispatchers( IN PDRIVER_OBJECT DriverObject );

static UNICODE_STRING SymbolicLinkName;

#endif // _KENTRY_H
