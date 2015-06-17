#ifndef _KREGISTRATION_H
#define _KREGISTRATION_H

#include <fltKernel.h>
#include <ntstrsafe.h>
#include <wdm.h>
#include "kcallbacks.h"

static CONST FLT_OPERATION_REGISTRATION Callbacks[] =
{
    {IRP_MJ_CREATE, 0, PreOperationCallback, NULL},

    {IRP_MJ_OPERATION_END}
};

static FLT_REGISTRATION FilterRegistration =
{
    sizeof(FLT_REGISTRATION),
    FLT_REGISTRATION_VERSION,
    0,
    NULL,
    Callbacks,
    FilterUnload,
    FilterLoad,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL
};

#endif // _KREGISTRATION_H

