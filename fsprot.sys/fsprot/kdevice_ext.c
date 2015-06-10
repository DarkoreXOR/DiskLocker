#include "kdevice_ext.h"

void SetDeviceExt( PDEVICE_EXT_DATA data )
{
    DeviceExt = data;
}

PDEVICE_EXT_DATA GetDeviceExt()
{
    return DeviceExt;
}

void SetDriverUnloadAccess( BOOLEAN CanUnload)
{
    if (CanUnload)
    {
        GetDeviceExt()->DriverObject->DriverUnload = GetDeviceExt()->UnloadRoutine;
    }
    else
    {
        GetDeviceExt()->DriverObject->DriverUnload = NULL;
    }
}

BOOLEAN CheckIRQL( KIRQL needIrql )
{
    KIRQL currentIrql = KeGetCurrentIrql();

    if (currentIrql <= needIrql)
    {
        return TRUE;
    }

    return FALSE;
}

PFILE_OBJECT_LIST_ENTRY K_InitializeList( PFILE_OBJECT_LIST_ENTRY *head )
{
    if (head == NULL)
    {
        return NULL;
    }

    *head = ALLOC_LIST_ENTRY( KFILE_OBJECT_LIST_ENTRY );

    if (*head != NULL)
    {
        (*head)->next = NULL;
        (*head)->prev = NULL;
        return *head;
    }
    return NULL;
}

PFILE_OBJECT_LIST_ENTRY K_InsertLast( PFILE_OBJECT_LIST_ENTRY *head )
{
    PFILE_OBJECT_LIST_ENTRY current = *head;
    PFILE_OBJECT_LIST_ENTRY prev = current;

    if (head == NULL)
    {
        return NULL;
    }

    if (*head == NULL)
    {
        return K_InitializeList( head );
    }

    while (current->next != NULL)
    {
        current = current->next;
        prev = current;
    }

    PFILE_OBJECT_LIST_ENTRY entry = ALLOC_LIST_ENTRY( KFILE_OBJECT_LIST_ENTRY );

    if (entry == NULL)
    {
        return NULL;
    }

    entry->next = NULL;
    entry->prev = prev;

    current->next = entry;

    return entry;
}

PFILE_OBJECT_LIST_ENTRY K_InsertFirst( PFILE_OBJECT_LIST_ENTRY *head )
{
    if (head == NULL)
    {
        return NULL;
    }

    if (*head == NULL)
    {
        return K_InitializeList( head );
    }

    PFILE_OBJECT_LIST_ENTRY entry = ALLOC_LIST_ENTRY( KFILE_OBJECT_LIST_ENTRY );

    if (entry == NULL)
    {
        return NULL;
    }

    entry->next = *head;
    entry->prev = NULL;

    (*head)->prev = entry;
    *head = entry;

    return entry;
}

BOOLEAN K_RemoveFirst( PFILE_OBJECT_LIST_ENTRY *head )
{
    if (head == NULL)
    {
        return FALSE;
    }

    if (*head == NULL)
    {
        return FALSE;
    }

    PFILE_OBJECT_LIST_ENTRY current = *head;

    *head = (*head)->next;

    if (*head != NULL)
    {
        (*head)->prev = NULL;
    }

    DEALLOC_LIST_ENTRY( current );

    return TRUE;
}

BOOLEAN K_RemoveLast( PFILE_OBJECT_LIST_ENTRY *head )
{
    if (head == NULL)
    {
        return FALSE;
    }

    if (*head == NULL)
    {
        return FALSE;
    }

    PFILE_OBJECT_LIST_ENTRY current = *head;

    if (current->next == NULL)
    {

        DEALLOC_LIST_ENTRY( *head );
        *head = NULL;
        return TRUE;
    }

    while (current->next != NULL)
    {
        current = current->next;
    }

    if (current->prev != NULL)
    {
        current->prev->next = NULL;
    }

    DEALLOC_LIST_ENTRY( current );

    return TRUE;
}

BOOLEAN K_RemoveEntry( PFILE_OBJECT_LIST_ENTRY *head, PFILE_OBJECT_LIST_ENTRY entry )
{
    if (head == NULL)
    {
        return FALSE;
    }

    if (*head == NULL)
    {
        return FALSE;
    }

    if (*head == entry)
    {
        return K_RemoveFirst( head );
    }

    PFILE_OBJECT_LIST_ENTRY current = *head;
    PFILE_OBJECT_LIST_ENTRY removeEntry = NULL;

    while (current != NULL)
    {
        current = current->next;

        if (current == entry)
        {
            removeEntry = current;
            break;
        }
    }

    if (removeEntry == NULL)
    {
        return FALSE;
    }

    PFILE_OBJECT_LIST_ENTRY next = removeEntry->next;
    PFILE_OBJECT_LIST_ENTRY prev = removeEntry->prev;

    if (next != NULL)
    {
        next->prev = prev;
    }

    if (prev != NULL)
    {
        prev->next = next;
    }

    DEALLOC_LIST_ENTRY( removeEntry );
    return TRUE;
}

PFILE_OBJECT_LIST_ENTRY K_GetNextEntry( PFILE_OBJECT_LIST_ENTRY entry )
{
    if (entry == NULL)
    {
        return NULL;
    }

    return entry->next;
}

PFILE_OBJECT_LIST_ENTRY K_GetPrevEntry( PFILE_OBJECT_LIST_ENTRY entry )
{
    if (entry == NULL)
    {
        return NULL;
    }

    return entry->prev;
}

BOOLEAN K_ReleaseList( PFILE_OBJECT_LIST_ENTRY *head )
{
    if (head == NULL)
    {
        return FALSE;
    }

    if (*head == NULL)
    {
        return FALSE;
    }

    while (*head != NULL)
    {
        K_RemoveFirst( head );
    }

    return TRUE;
}

BOOLEAN K_ContainsEntry( PFILE_OBJECT_LIST_ENTRY head, PFILE_OBJECT_LIST_ENTRY entry )
{
    if (head == NULL)
    {
        return FALSE;
    }

    if (entry == NULL)
    {
        return FALSE;
    }

    PFILE_OBJECT_LIST_ENTRY last = head;

    while (last != NULL)
    {
        if (last == entry)
        {
            return TRUE;
        }
        last = K_GetNextEntry( last );
    }

    return FALSE;
}

int K_GetListCount( PFILE_OBJECT_LIST_ENTRY head )
{
    if (head == NULL)
    {
        return 0;
    }

    PFILE_OBJECT_LIST_ENTRY current = head;

    int count = 0;

    do
    {
        count++;
    }
    while ((current = K_GetNextEntry( current )) != NULL);

    return count;
}
