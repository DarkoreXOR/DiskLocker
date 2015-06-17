#ifndef _KCALLBACKS_H
#define _KCALLBACKS_H

#include <fltKernel.h>
#include <ntstrsafe.h>
#include <wdm.h>
#include "kdevice_ext.h"

BOOLEAN IsCorrectFileName( IN PUNICODE_STRING filename );

BOOLEAN PreOperationChecks( IN OUT PFLT_CALLBACK_DATA Data,
                            IN PCFLT_RELATED_OBJECTS FltObjects );

FLT_PREOP_CALLBACK_STATUS PreOperationCallback(
    IN OUT PFLT_CALLBACK_DATA Data,
    IN PCFLT_RELATED_OBJECTS FltObjects,
    IN OUT PVOID *CompletionContext );

FLT_POSTOP_CALLBACK_STATUS PostOperationCallback(
    IN OUT PFLT_CALLBACK_DATA Data,
    IN PCFLT_RELATED_OBJECTS FltObjects,
    IN PVOID CompletionContext,
    IN FLT_POST_OPERATION_FLAGS Flags );

NTSTATUS FilterLoad( IN PCFLT_RELATED_OBJECTS  FltObjects,
                     IN FLT_INSTANCE_SETUP_FLAGS  Flags,
                     IN DEVICE_TYPE  VolumeDeviceType,
                     IN FLT_FILESYSTEM_TYPE  VolumeFilesystemType );

NTSTATUS FilterUnload( IN FLT_FILTER_UNLOAD_FLAGS Flags );

#endif // _KCALLBACKS_H
