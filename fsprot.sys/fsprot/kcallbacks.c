#include "kcallbacks.h"

BOOLEAN IsCorrectFileName( IN PUNICODE_STRING filename )
{
    if (filename == NULL)
    {
        return FALSE;
    }

    if (filename->Length == 0)
    {
        return FALSE;
    }

    if (filename->Buffer == NULL)
    {
        return FALSE;
    }

    return TRUE;
}

BOOLEAN PreOperationChecks( IN OUT PFLT_CALLBACK_DATA Data,
                            IN PCFLT_RELATED_OBJECTS FltObjects )
{
    if (FLT_IS_FS_FILTER_OPERATION( Data ))
    {
        return FALSE;
    }

    if (FltObjects->FileObject == NULL || Data == NULL)
    {
        return FALSE;
    }

    if (Data->Iopb->TargetFileObject == NULL)
    {
        return FALSE;
    }

    if (Data->Iopb->MajorFunction != IRP_MJ_CREATE)
    {
        return FALSE;
    }

    return TRUE;
}

FLT_PREOP_CALLBACK_STATUS PreOperationCallback( IN OUT PFLT_CALLBACK_DATA Data,
                                                IN PCFLT_RELATED_OBJECTS FltObjects,
                                                IN OUT PVOID *CompletionContext )
{
    UNREFERENCED_PARAMETER( Data );
    UNREFERENCED_PARAMETER( FltObjects );
    UNREFERENCED_PARAMETER( CompletionContext );

    if (!CheckIRQL( APC_LEVEL ))
    {
        return FLT_PREOP_SUCCESS_NO_CALLBACK;
    }

    if (!PreOperationChecks( Data, FltObjects ))
    {
        return FLT_PREOP_SUCCESS_NO_CALLBACK;
    }

    PFLT_FILE_NAME_INFORMATION FileInformation;

    NTSTATUS status = FltGetFileNameInformation(
        Data,
        FLT_FILE_NAME_NORMALIZED |
        FLT_FILE_NAME_QUERY_ALWAYS_ALLOW_CACHE_LOOKUP,
        &FileInformation );

    if (!NT_SUCCESS( status ))
    {
        return FLT_PREOP_SUCCESS_NO_CALLBACK;
    }

    if (FileInformation == NULL)
    {
        return FLT_PREOP_SUCCESS_NO_CALLBACK;
    }

    if (!IsCorrectFileName( &(Data->Iopb->TargetFileObject->FileName) ))
    {
        FltReleaseFileNameInformation( FileInformation );
        return FLT_PREOP_SUCCESS_NO_CALLBACK;
    }

    if (!IsCorrectFileName( &(FileInformation->Volume) ))
    {
        FltReleaseFileNameInformation( FileInformation );
        return FLT_PREOP_SUCCESS_NO_CALLBACK;
    }

    KIRQL oldIrql;

    KeAcquireSpinLock( &(GetDeviceExt()->FileListAccessSpinLock), &oldIrql );

    BOOLEAN IsProtected = FALSE;

    /* IRQL == DISPATCH_LEVEL */

    PFILE_OBJECT_LIST_ENTRY head = GetDeviceExt()->FileListHead;

    while (head != NULL)
    {
        WCHAR * path = head->fullPath;
        WCHAR * targetPath = FileInformation->Volume.Buffer;

        WCHAR upperPath[1024 * 4];
        WCHAR upperTargetPath[1024 * 4];

        wcscpy( upperPath, path );
        wcscpy( upperTargetPath, targetPath );

        _wcsupr( upperPath );
        _wcsupr( upperTargetPath );

        if (head->weakProtection)
        {
            if (wcscmp( upperTargetPath, upperPath ) == 0)
            {
                IsProtected = TRUE;
                break;
            }
        }
        else
        {
            if (wcsstr( upperTargetPath, upperPath ) != NULL)
            {
                IsProtected = TRUE;
                break;
            }
        }

        head = K_GetNextEntry( head );
    }

    KeReleaseSpinLock( &(GetDeviceExt()->FileListAccessSpinLock), oldIrql );

    FltReleaseFileNameInformation( FileInformation );

    if (IsProtected)
    {
        Data->IoStatus.Status = STATUS_ACCESS_DENIED;
        return FLT_PREOP_COMPLETE;
    }

    return FLT_PREOP_SUCCESS_NO_CALLBACK;
}

FLT_POSTOP_CALLBACK_STATUS PostOperationCallback(
    IN OUT PFLT_CALLBACK_DATA Data,
    IN PCFLT_RELATED_OBJECTS FltObjects,
    IN PVOID CompletionContext,
    IN FLT_POST_OPERATION_FLAGS Flags )
{
    /* IRQL == PASSIVE_LEVEL */

    UNREFERENCED_PARAMETER( Data );
    UNREFERENCED_PARAMETER( FltObjects );
    UNREFERENCED_PARAMETER( CompletionContext );
    UNREFERENCED_PARAMETER( Flags );

    DbgPrint( "PostOperationCallback()" );

    return FLT_POSTOP_FINISHED_PROCESSING;
}

NTSTATUS FilterLoad( IN PCFLT_RELATED_OBJECTS FltObjects,
                     IN FLT_INSTANCE_SETUP_FLAGS Flags,
                     IN DEVICE_TYPE VolumeDeviceType,
                     IN FLT_FILESYSTEM_TYPE VolumeFilesystemType )
{
    /* IRQL == PASSIVE_LEVEL */

    UNREFERENCED_PARAMETER( FltObjects );
    UNREFERENCED_PARAMETER( Flags );
    UNREFERENCED_PARAMETER( VolumeDeviceType );
    UNREFERENCED_PARAMETER( VolumeFilesystemType );

    if (VolumeDeviceType == FILE_DEVICE_NETWORK_FILE_SYSTEM)
    {
        return STATUS_FLT_DO_NOT_ATTACH;
    }

    return STATUS_SUCCESS;
}

NTSTATUS FilterUnload( IN FLT_FILTER_UNLOAD_FLAGS Flags )
{
    /* IRQL == PASSIVE_LEVEL */

    UNREFERENCED_PARAMETER( Flags );

    DbgPrint( "file system filter unloading..." );

    KIRQL oldIrql;

    KeAcquireSpinLock( &(GetDeviceExt()->FileListAccessSpinLock), &oldIrql );

    K_ReleaseList( &(GetDeviceExt()->FileListHead) );

    KeReleaseSpinLock( &(GetDeviceExt()->FileListAccessSpinLock), oldIrql );

    /* unregister filter */
    FltUnregisterFilter( GetDeviceExt()->FltFilter );

    return STATUS_SUCCESS;
}

