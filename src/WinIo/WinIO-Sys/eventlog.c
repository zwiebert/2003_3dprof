#include <ntddk.h>
#include <string.h>
#include <stdio.h>

/**********************************************************
*
* Function   :  SstReportEvent
*
* Description:  Uses the input parameters to send a message to the Event Log
*               found in the Administrative Tools Section of Windows NT.
*
*******************************************************/

#define EVENTLOG_MSG_ARR_SIZE 40

LONG
SstReportEvent(IN PVOID pIoObject, IN NTSTATUS MsgCode, IN CHAR *p_c_ArgString)
{
  PIO_ERROR_LOG_PACKET pPacket;
  UCHAR uc_Size;
  WCHAR arr_wd_EventLogMsg[EVENTLOG_MSG_ARR_SIZE]; 

  /* this EVENTLOG_MSG_ARR_SIZE equals 41 */

  if ((pIoObject == NULL) || (p_c_ArgString == NULL))
    return -1;

  if ((strlen(p_c_ArgString) <= 0) || (strlen(p_c_ArgString) >= EVENTLOG_MSG_ARR_SIZE))
    return -2;

  swprintf(arr_wd_EventLogMsg, L"%S", p_c_ArgString);

  uc_Size = sizeof(IO_ERROR_LOG_PACKET)
    + ((wcslen(arr_wd_EventLogMsg) + 1) * sizeof(WCHAR));

  if (uc_Size >= ERROR_LOG_MAXIMUM_SIZE)
    return -3;


  // Try to allocate the packet
  pPacket = IoAllocateErrorLogEntry(pIoObject, uc_Size);

  if (pPacket == NULL)
    return -4;

  // Fill in standard parts of the packet
  pPacket->MajorFunctionCode     = 0;
  pPacket->RetryCount            = 0;
  pPacket->DumpDataSize          = 0;
  pPacket->EventCategory         = 0;
  pPacket->ErrorCode             = MsgCode;
  pPacket->UniqueErrorValue      = 0;
  pPacket->FinalStatus           = STATUS_SUCCESS;
  pPacket->SequenceNumber        = 0;
  pPacket->IoControlCode         = 0;
  pPacket->DeviceOffset.QuadPart = 0;
  pPacket->NumberOfStrings       = 1;
  pPacket->StringOffset          = FIELD_OFFSET(IO_ERROR_LOG_PACKET, DumpData) ;

  RtlCopyMemory((PWSTR) (&pPacket->DumpData[0]), arr_wd_EventLogMsg, uc_Size - sizeof(IO_ERROR_LOG_PACKET));


  // Log the message
  IoWriteErrorLogEntry(pPacket);

  return 0;

}

