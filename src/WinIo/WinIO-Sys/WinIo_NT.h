#ifndef WINIONT_H
#define WINIONT_H

// Define the various device type values.  Note that values used by Microsoft
// Corporation are in the range 0-32767, and 32768-65535 are reserved for use
// by customers.

#define FILE_DEVICE_WINIO 0x00008010

// Macro definition for defining IOCTL and FSCTL function control codes.
// Note that function codes 0-2047 are reserved for Microsoft Corporation,
// and 2048-4095 are reserved for customers.

#define WINIO_IOCTL_INDEX 0x810

// Define our own private IOCTL

#define IOCTL_WINIO_MAPPHYSTOLIN     CTL_CODE(FILE_DEVICE_WINIO,  \
                                     WINIO_IOCTL_INDEX + 0,      \
                                     METHOD_BUFFERED,        \
                                     FILE_ANY_ACCESS)

#define IOCTL_WINIO_UNMAPPHYSADDR    CTL_CODE(FILE_DEVICE_WINIO,  \
                                     WINIO_IOCTL_INDEX + 1,  \
                                     METHOD_BUFFERED,        \
                                     FILE_ANY_ACCESS)

#define IOCTL_WINIO_ENABLEDIRECTIO   CTL_CODE(FILE_DEVICE_WINIO,  \
                                     WINIO_IOCTL_INDEX + 2,   \
                                     METHOD_BUFFERED,         \
                                     FILE_ANY_ACCESS)

#define IOCTL_WINIO_DISABLEDIRECTIO  CTL_CODE(FILE_DEVICE_WINIO,  \
                                     WINIO_IOCTL_INDEX + 3,   \
                                     METHOD_BUFFERED,         \
                                     FILE_ANY_ACCESS)

#define IOCTL_WINIO_TEST             CTL_CODE(FILE_DEVICE_WINIO, \
                                     WINIO_IOCTL_INDEX + 4,   \
                                     METHOD_BUFFERED,         \
                                     FILE_ANY_ACCESS)

#define IOCTL_WINIO_MAPIO            CTL_CODE(FILE_DEVICE_WINIO,  \
                                     WINIO_IOCTL_INDEX + 5,      \
                                     METHOD_BUFFERED,        \
                                     FILE_ANY_ACCESS)

#define IOCTL_WINIO_UNMAPIO          CTL_CODE(FILE_DEVICE_WINIO,  \
                                     WINIO_IOCTL_INDEX + 6,  \
                                     METHOD_BUFFERED,        \
                                     FILE_ANY_ACCESS)

#define IOCTL_WINIO_GETPCICOMMHDR           CTL_CODE(FILE_DEVICE_WINIO,  \
                                     WINIO_IOCTL_INDEX + 7,  \
                                     METHOD_BUFFERED,        \
                                     FILE_ANY_ACCESS)


#pragma pack(1)

struct tagPhys32Struct
{
  HANDLE PhysicalMemoryHandle;
  ULONG dwPhysMemSizeInBytes;
  PVOID pvPhysAddress;
  PVOID pvPhysMemLin;
};

struct winio_map_io_data {
  ULONG size_requested;
  ULONG phys_addr_requested;
  PVOID virt_kaddr;
  PVOID virt_uaddr;
  PVOID mdl;
};


  struct winio_pci_common_header_ {
    USHORT  Bus;   //Bus number
    USHORT  Slot;  //Dev+Fn=virtual slot number
    USHORT  Dev;
    USHORT  Fn;

    USHORT  VendorID;                   // (ro)
    USHORT  DeviceID;                   // (ro)
    USHORT  Command;                    // Device control
    USHORT  Status;
    UCHAR   RevisionID;                 // (ro)
    UCHAR   ProgIf;                     // (ro)
    UCHAR   SubClass;                   // (ro)
    UCHAR   BaseClass;                  // (ro)
    UCHAR   CacheLineSize;              // (ro+)
    UCHAR   LatencyTimer;               // (ro+)
    UCHAR   HeaderType;                 // (ro)
    UCHAR   BIST;                       // Built in self test

    ULONG   BaseAddresses[6];
    ULONG   CIS;
    USHORT  SubVendorID;
    USHORT  SubSystemID;
    ULONG   ROMBaseAddress;
    UCHAR   CapabilitiesPtr;
    UCHAR   Reserved1[3];
    ULONG   Reserved2;
    UCHAR   InterruptLine;      //
    UCHAR   InterruptPin;       // (ro)
    UCHAR   MinimumGrant;       // (ro)
    UCHAR   MaximumLatency;     // (ro)
  };

extern struct tagPhys32Struct Phys32Struct;

#endif
