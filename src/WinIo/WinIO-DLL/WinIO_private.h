#ifndef WINIO_PRIVATE_H_
#define WINIO_PRIVATE_H_
#include "WinIO.h"

/*
There should be two different Service Names:

One name for driver which are installed permanently and are loaded at system start.

A second name (DRV_SERVICE_NAME) for demand loading.
 
*/

#define DRV_SERVICE_NAME "NvcWioDemand"
#define DRV_SERVICE_DISPLAY "NvcWioDemand"
#define DRV_DEVICE_NAME "\\\\.\\" "NVC_WINIO" // must be the same string as in driver source
#define DRV_BINARY_NAME "winio.sys"


typedef struct winio_instance {
  int handles_nmb;
  int handles_size;
  struct handle *handles;
} *WinIO_Inst;

typedef void *WinIO;

extern BOOL IsNT;
extern HANDLE hDriver;
extern BOOL IsWinIoInitialized;


BOOL _stdcall StartWinIoDriver();
BOOL _stdcall StopWinIoDriver();


#endif /* WINIO_PRIVATE_H_ */
