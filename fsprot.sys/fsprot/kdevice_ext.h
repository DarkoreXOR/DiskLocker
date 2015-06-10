#ifndef _KDEVICE_EXT_H
#define _KDEVICE_EXT_H

#include <fltKernel.h>
#include <ntstrsafe.h>
#include <wdm.h>

#define ALLOC_LIST_ENTRY( type ) ( ExAllocatePool( NonPagedPool, sizeof( type ) ) )
#define DEALLOC_LIST_ENTRY( p ) { if( p != NULL ) ExFreePool( p ); p = NULL; }

typedef struct _FILE_OBJECT_LIST_ENTRY KFILE_OBJECT_LIST_ENTRY;

typedef struct _FILE_OBJECT_LIST_ENTRY
{
    // list navigation
    KFILE_OBJECT_LIST_ENTRY *next;
    KFILE_OBJECT_LIST_ENTRY *prev;

    // key for application action
    DWORD uniqueKey;

    // protection type
    BOOLEAN weakProtection;

    // file object full path
    WCHAR fullPath[1024];

} KFILE_OBJECT_LIST_ENTRY, *PFILE_OBJECT_LIST_ENTRY;

typedef struct _DEVICE_EXT_DATA
{
    /* auto-increment unique key */
    DWORD LastUniqueKey;

    /* filter */
    PFLT_FILTER FltFilter;

    /* for device internal functions */
    KSPIN_LOCK InternalDeviceSyncSpinLock;

    /* device for app I/O */
    PDEVICE_OBJECT DeviceObject;

    /* driver for unload access */
    PDRIVER_OBJECT DriverObject;

    /* driver unload routine */
    PDRIVER_UNLOAD UnloadRoutine;

    /* data list */
    PFILE_OBJECT_LIST_ENTRY FileListHead;

    /* for list operations synchronization */
    KSPIN_LOCK FileListAccessSpinLock;

} DEVICE_EXT_DATA, *PDEVICE_EXT_DATA;

static PDEVICE_EXT_DATA DeviceExt;

void SetDeviceExt( PDEVICE_EXT_DATA data );
PDEVICE_EXT_DATA GetDeviceExt();

void SetDriverUnloadAccess( BOOLEAN CanUnload );

BOOLEAN CheckIRQL( KIRQL needIrql );

PFILE_OBJECT_LIST_ENTRY K_InitializeList( PFILE_OBJECT_LIST_ENTRY *head );
PFILE_OBJECT_LIST_ENTRY K_InsertLast( PFILE_OBJECT_LIST_ENTRY *head );
PFILE_OBJECT_LIST_ENTRY K_InsertFirst( PFILE_OBJECT_LIST_ENTRY *head );
BOOLEAN K_RemoveFirst( PFILE_OBJECT_LIST_ENTRY *head );
BOOLEAN K_RemoveLast( PFILE_OBJECT_LIST_ENTRY *head );
BOOLEAN K_RemoveEntry( PFILE_OBJECT_LIST_ENTRY *head, PFILE_OBJECT_LIST_ENTRY entry );
PFILE_OBJECT_LIST_ENTRY K_GetNextEntry( PFILE_OBJECT_LIST_ENTRY entry );
PFILE_OBJECT_LIST_ENTRY K_GetPrevEntry( PFILE_OBJECT_LIST_ENTRY entry );
BOOLEAN K_ReleaseList( PFILE_OBJECT_LIST_ENTRY *head );
BOOLEAN K_ContainsEntry( PFILE_OBJECT_LIST_ENTRY head, PFILE_OBJECT_LIST_ENTRY entry );

int K_GetListCount( PFILE_OBJECT_LIST_ENTRY head );


#endif // _KDEVICE_EXT_H
