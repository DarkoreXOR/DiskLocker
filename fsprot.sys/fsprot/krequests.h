#ifndef _KREQUESTS_H
#define _KREQUESTS_H

#include <fltKernel.h>
#include <ntstrsafe.h>
#include <wdm.h>
#include "kdevice_ext.h"

/* add path for protection */
typedef struct _KADD_PATH_FOR_PROTECTION_IN_PACKET
{
    WCHAR FullPath[1024];
    BOOLEAN WeakProtection;
} KADD_PATH_FOR_PROTECTION_IN_PACKET, *PKADD_PATH_FOR_PROTECTION_IN_PACKET;

typedef struct _KADD_PATH_FOR_PROTECTION_OUT_PACKET
{
    DWORD UniqueKey;

} KADD_PATH_FOR_PROTECTION_OUT_PACKET, *PKADD_PATH_FOR_PROTECTION_OUT_PACKET;

/* remove path from protection */

typedef struct _KREMOVE_PATH_FROM_PROTECTION_IN_PACKET
{
    DWORD UniqueKey;
} KREMOVE_PATH_FROM_PROTECTION_IN_PACKET, *PKREMOVE_PATH_FROM_PROTECTION_IN_PACKET;

typedef struct _KREMOVE_PATH_FROM_PROTECTION_OUT_PACKET
{
    BOOLEAN RemoveResult;
} KREMOVE_PATH_FROM_PROTECTION_OUT_PACKET, *PKREMOVE_PATH_FROM_PROTECTION_OUT_PACKET;

/* driver unload protection */

typedef struct _KSET_UNLOAD_ACCESS_IN_PACKET
{
    BOOLEAN CanUnload;
} KSET_UNLOAD_ACCESS_IN_PACKET, *PKSET_UNLOAD_ACCESS_IN_PACKET;

NTSTATUS AddPathForProtection(
    PVOID InputBuffer,
    DWORD InputBufferSize,
    PVOID OutputBuffer,
    DWORD OutputBufferSize );

NTSTATUS RemovePathFromProtection(
    PVOID InputBuffer,
    DWORD InputBufferSize,
    PVOID OutputBuffer,
    DWORD OutputBufferSize );

NTSTATUS RemoveAllPathsFromProtection(
    PVOID InputBuffer,
    DWORD InputBufferSize,
    PVOID OutputBuffer,
    DWORD OutputBufferSize );

NTSTATUS SetUnloadAccess(
    PVOID InputBuffer,
    DWORD InputBufferSize,
    PVOID OutputBuffer,
    DWORD OutputBufferSize );

#endif // _KREQUESTS_H

