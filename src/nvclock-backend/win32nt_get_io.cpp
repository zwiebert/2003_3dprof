#include "win32nt_get_io.h"

#if USE_SETUPAPI_DLL

#define _WIN32_WINNT 0x0400
#include <windows.h>
#include <Setupapi.h>
#include <cfgmgr32.h>
#include <stdio.h>
#if 0
# define D(x) x
#else
# define D(x)
#endif

//GUID display_guid = "{4D36E968-E325-11CE-BFC1-08002BE10318}";
static void init_module();

static GUID display_guid = {0x4D36E968, 0xE325, 0x11CE, {0xBF, 0xC1, 0x08, 0x00, 0x2B, 0xE1, 0x03, 0x18}};
static unsigned long mem_base[10];
static unsigned long mem_size[10];
static int io_base[10];
static int io_size[10];
static unsigned short device_id;

unsigned long get_mem_base(unsigned idx) {
  init_module();
  if (idx < 10)
    return mem_base[idx];
  return 0;
}
unsigned long get_mem_size(unsigned idx) {
  init_module();
  if (idx < 10)
    return mem_size[idx];
  return 0;
}

unsigned short get_device_id() {
  init_module();
  return device_id;
}

static void PrintToFile(char *szDataString)
{
#ifdef DEBUG
  printf("%s", szDataString);
#else
  (void)szDataString;
#endif
}

static void print_error()
{
  LPVOID lpMsgBuf;
  FormatMessage( 
    FORMAT_MESSAGE_ALLOCATE_BUFFER | 
    FORMAT_MESSAGE_FROM_SYSTEM | 
    FORMAT_MESSAGE_IGNORE_INSERTS,
    NULL,
    GetLastError(),
    MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), // Default language
    (LPTSTR) &lpMsgBuf,
    0,
    NULL 
    );
  //fprintf (stderr, "error: %s\n", lpMsgBuf);
  // Process any inserts in lpMsgBuf.
  // ...
  // Display the string.
  MessageBox( NULL, (LPCTSTR)lpMsgBuf, "Error", MB_OK | MB_ICONINFORMATION );
  // Free the buffer.
  LocalFree( lpMsgBuf );
}

static int f()
{ 
  bool niRetour = FALSE;
  HDEVINFO hDevInfo = 0;
  SP_DEVINFO_DATA DeviceInfoData = {sizeof(SP_DEVINFO_DATA)};	
  SP_PROPCHANGE_PARAMS PropChangeParams = {sizeof	(SP_CLASSINSTALL_HEADER)};
  ULONG Length = 1024 * sizeof (TCHAR);
  TCHAR *buffer = (TCHAR *) LocalAlloc(LPTR, Length);
  IO_RESOURCE **result = 0;
  int found_io_res = 0;


  if ((hDevInfo = SetupDiGetClassDevs(&display_guid, NULL, NULL,
    (/* DIGCF_ALLCLASSES | */ DIGCF_PRESENT | DIGCF_PROFILE))) != INVALID_HANDLE_VALUE)
  {
    // enumeration
    for (DWORD niI=0; SetupDiEnumDeviceInfo(hDevInfo, niI, &DeviceInfoData); niI++)
    {
      Length = MAX_PATH;
      if (SetupDiGetDeviceRegistryProperty(hDevInfo, &DeviceInfoData, SPDRP_DEVICEDESC, 
        NULL, (PBYTE) buffer, Length, 0))
      {
#ifdef DEBUG
        printf("Device=\"%s\"\n", buffer);
#endif
        if (true || DeviceInfoData.ClassGuid == display_guid)
        {
          LOG_CONF log_conf=0;
          RES_DES out_res_des=0;
          RESOURCEID res_type = ResType_Mem; //ResType_All
          RESOURCEID out_res_type; // only used with ResType_All as input
          CONFIGRET cr = CM_Get_First_Log_Conf(&log_conf, DeviceInfoData.DevInst, ALLOC_LOG_CONF);
          unsigned mem_idx = 0;

          if (cr != CR_SUCCESS) {
            printf("cr_error=0x%08x\n", cr);
          } else {
            RES_DES res_des = log_conf;
            while (CR_SUCCESS == (cr = CM_Get_Next_Res_Des(&out_res_des, res_des, res_type, &out_res_type, 0))) {
              ULONG size;
              if (CR_SUCCESS == CM_Get_Res_Des_Data_Size(&size, out_res_des, 0)) {
                ULONG test_size = sizeof (IO_RESOURCE);
                void *buf = malloc(size);
                if (CR_SUCCESS == CM_Get_Res_Des_Data(out_res_des, buf, size, 0)) {
                  void *Buffer = buf;


                  if (res_type == ResType_Mem || out_res_type == ResType_Mem) {
                    MEM_RESOURCE *PMEM_Resource = (MEM_RESOURCE *)(Buffer);
                    char szBuffer[LINE_LEN];                                              
                    ULONG Mem_base = (ULONG)PMEM_Resource->MEM_Header.MD_Alloc_Base;
                    ULONG Mem_size = 1 + (ULONG)(PMEM_Resource->MEM_Header.MD_Alloc_End - Mem_base);
                    const char *size_unit = "byte";
                    if (Mem_size > (1024 * 1024)) {
                      Mem_size /= (1024 * 1024);
                      size_unit = "MB";
                    } else if (Mem_size > (1024)) {
                      Mem_size /= (1024);
                      size_unit = "KB";
                    } 

                    sprintf(szBuffer,
                      "    Memory Range         : %.8lX - %.8lX (%d %s) MD_Flags: 0x%08lx\n", 
                      Mem_base, (ULONG)PMEM_Resource->MEM_Header.MD_Alloc_End,
                      Mem_size, size_unit, (ULONG)PMEM_Resource->MEM_Header.MD_Flags);
                    PrintToFile(szBuffer);

                    if (mem_idx < 10) {
                      mem_base[mem_idx] = Mem_base;
                      mem_size[mem_idx] = Mem_size;
                      mem_idx++;
                    }
                  }


                  if (res_type == ResType_IO || out_res_type == ResType_IO) {
                    IO_RESOURCE *PIO_Resource = (IO_RESOURCE *)(Buffer);

                    result = (PIO_RESOURCE *)realloc(result, sizeof(PIO_RESOURCE) * ++found_io_res);
                    result[found_io_res-1] = PIO_Resource;


                    printf("    IO Range             : %04X - %04X\n", 
                      (int)PIO_Resource->IO_Header.IOD_Alloc_Base, 
                      (int)PIO_Resource->IO_Header.IOD_Alloc_End);
                  }
                }
              }
              res_des = out_res_des;
            }

            CM_Free_Res_Des_Handle(out_res_des);
            CM_Free_Log_Conf_Handle(log_conf);
          }
        }
      }
    }
    SetupDiDestroyDeviceInfoList(hDevInfo);
  }

  if (buffer)
    LocalFree(buffer);

  return found_io_res;
}

static void init_device_id()
{
  DISPLAY_DEVICE dd;
  dd.cb = sizeof dd;

  if (EnumDisplayDevices(NULL, 0, &dd, 0)) {
    const char *token[10] = {0, };
    CHAR *dk = dd.DeviceKey;
    const char *pci = strtok(dd.DeviceID, "\\");
    for (int i=0; i < 10; ++i) {
      const char *tok = token[i] = strtok(NULL, "&");
      if (tok && toupper(tok[0]) == 'D' && toupper(tok[1]) == 'E' && toupper(tok[2]) == 'V'
        && tok[3] == '_') {
          device_id = (unsigned short)strtoul(tok + 4, (char **)0, 16);
        }
    }
  }
}


static void init_module()
{
  static BOOL initialized;
  if (!initialized) {
#if 1
    D(fputs("f() invoked\n", stderr));
    f();
    D(fputs("f() returned\n", stderr));
#else 
    mem_base[0] = 0xDE000000;
    mem_size[0] = 16 * 1024 * 1024;
#endif
    init_device_id();
    initialized = TRUE;
  }
}


#endif /* USE_SETUPAPI_DLL */
