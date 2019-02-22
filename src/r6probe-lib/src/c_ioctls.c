#include <r6probe.h>
#include <windows.h>
#include <r6probe_ioc.h>

HANDLE r6probe_hProbe;

unsigned __cdecl r6probe_readbios(unsigned addr)
{   
  DWORD bytes_returned;
  DeviceIoControl(r6probe_hProbe, IOCTL_R6_READ_BIOS, &addr, sizeof addr, &addr, sizeof addr, &bytes_returned, 0);
  return addr;
}

unsigned __cdecl r6probe_readmmr(unsigned addr)
{   
  DWORD bytes_returned;
  DeviceIoControl(r6probe_hProbe, IOCTL_R6_READ_MMR, &addr, sizeof addr, &addr, sizeof addr, &bytes_returned, 0);
  return addr;
}

unsigned __cdecl r6probe_readpll(unsigned addr)
{
  DWORD bytes_returned;
  DeviceIoControl(r6probe_hProbe, IOCTL_R6_READ_PLL, &addr, sizeof addr, &addr, sizeof addr, &bytes_returned, 0);
  return addr;
}

unsigned __cdecl r6probe_readpci(unsigned addr)
{
  DWORD bytes_returned;
  DeviceIoControl(r6probe_hProbe, IOCTL_R6_READ_PCI, &addr, sizeof addr, &addr, sizeof addr, &bytes_returned, 0);
  return addr;
}

unsigned __cdecl r6probe_readbiosblk(void* dest, unsigned addr, unsigned count)
{
  DWORD bytes_returned;
  DeviceIoControl(r6probe_hProbe, IOCTL_R6_READ_BIOS, &addr, sizeof addr, dest, count, &bytes_returned, 0);
  return bytes_returned;
}

unsigned __cdecl r6probe_getversion()
{
  DWORD bytes_returned;
  unsigned result = 0;
  DeviceIoControl(r6probe_hProbe, IOCTL_R6_GET_VERSION, NULL, 0, &result, sizeof result, &bytes_returned, 0);
  return result;
}


void __cdecl r6probe_writemmr(unsigned addr, unsigned mask, unsigned data)
{
  DWORD bytes_returned;
  DeviceIoControl(r6probe_hProbe, IOCTL_R6_WRITE_MMR, &addr, sizeof(MASKED_WRITE_PARAMS),
    NULL, 0, &bytes_returned, 0);
}

void __cdecl r6probe_writepll(unsigned addr, unsigned mask, unsigned data)
{
  DWORD bytes_returned;
  DeviceIoControl(r6probe_hProbe, IOCTL_R6_WRITE_PLL, &addr, sizeof(MASKED_WRITE_PARAMS),
    NULL, 0, &bytes_returned, 0);
}


void __cdecl r6probe_getadapterinfo(R6ADAPTER_INFO* ai)
{
  DWORD bytes_returned;
  DeviceIoControl(r6probe_hProbe, IOCTL_R6_GET_ADAPTER_INFO,
    NULL, 0,
    ai, sizeof(R6ADAPTER_INFO),
    &bytes_returned, 0);
}



