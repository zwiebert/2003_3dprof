#ifndef WINIO_PRIVATE_H_
#define WINIO_PRIVATE_H_
#include "WinIO.h"

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
