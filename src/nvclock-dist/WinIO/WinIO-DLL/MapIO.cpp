// ---------------------------------------------------- //
//                      WinIo v2.0                      //
//  Direct Hardware Access Under Windows 9x/NT/2000/XP  //
//           Copyright 1998-2002 Yariv Kaplan           //
//               http://www.internals.com               //
// ---------------------------------------------------- //

#include <windows.h>
#include <winioctl.h>
#include <winio_nt.h>
#include "WinIO.h"
#include "WinIO_private.h"


#define NMB_HANDLES 64
struct handle {
  PVOID usr_ptr;
  struct winio_map_io_data data;
};
#define handles (inst->handles)


static struct handle *
handle_get_empty(WinIO vp_inst)
{
  register struct winio_instance *inst = (struct winio_instance *)vp_inst;

  if (inst->handles_nmb <= inst->handles_size)  {
    if ((handles = ((struct handle *)realloc(handles, sizeof (struct handle) * (inst->handles_size + NMB_HANDLES) )))) {
      for (int i=0; i < NMB_HANDLES; ++i) {
        handles[inst->handles_size + i].usr_ptr = 0;
      }
      inst->handles_size += NMB_HANDLES;
    }
  }
  for (int i=0; i < inst->handles_size; ++i)
    if (handles[i].usr_ptr == 0) {
      ++inst->handles_nmb;
      return &handles[i];
    }

    return NULL;
}

static void handle_free(WinIO vp_inst, struct handle *h) {
  register struct winio_instance *inst = (struct winio_instance *)vp_inst;

  h->usr_ptr = 0;
  --inst->handles_nmb;
}

static struct handle *handle_find(WinIO vp_inst, PVOID usr_ptr) {
  register struct winio_instance *inst = (struct winio_instance *)vp_inst;

  for (int i=0; i < inst->handles_size; ++i)
    if (handles[i].usr_ptr == usr_ptr)
      return &handles[i];

  return NULL;
}

PVOID _stdcall MapIO(WinIO vp_inst, ULONG phys_addr, ULONG size)
{
  register struct winio_instance *inst = (struct winio_instance *)vp_inst;
  DWORD dwBytesReturned;
  struct handle *h;

  if (!IsWinIoInitialized)
    return NULL;
  if (!IsNT)
    return NULL;

  if (IsNT)
  {
    if (!(h = handle_get_empty(inst)))
      return NULL;

    h->data.size_requested = size;
    h->data.phys_addr_requested =  phys_addr;

    if (!DeviceIoControl(hDriver, IOCTL_WINIO_MAPIO, &h->data, sizeof h->data,
      &h->data, sizeof h->data, &dwBytesReturned, NULL))
      return NULL;
    else
    {
      h->usr_ptr = h->data.virt_uaddr;
      return h->data.virt_uaddr;
    }
  }
  else
  {
    return 0;
  }

  return 0;
}

VOID _stdcall UnMapIO(WinIO vp_inst, PVOID virt_uaddr)
{
  register struct winio_instance *inst = (struct winio_instance *)vp_inst;
  struct handle *h;
  DWORD dwBytesReturned;

  if (!IsNT)
    return;
  if (!virt_uaddr)
    return;
  if (!(h= handle_find(inst, virt_uaddr)))
    return;


  if (IsNT)
  {
    if (!IsWinIoInitialized)
      return;

    DeviceIoControl(hDriver, IOCTL_WINIO_UNMAPIO, &h->data, sizeof h->data,
      NULL, 0, &dwBytesReturned, NULL);
    handle_free(inst, h);
  }
}
