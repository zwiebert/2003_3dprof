// structures used in .ico files
#pragma pack(push)
#pragma pack(2)

// structure of icon-image, reffered by dwImageOffset in ICONDIRENTRY below
typedef struct
{
   BITMAPINFOHEADER icHeader;      // DIB header
   RGBQUAD          icColors[1];   // Color table
   BYTE             icXOR[1];      // DIB bits for XOR mask
   BYTE             icAND[1];      // DIB bits for AND mask
} ICONIMAGE, *PICONIMAGE;

// structure of idEntries member of ICONDIR below
typedef struct
{
    BYTE        bWidth;          // Width, in pixels, of the image
    BYTE        bHeight;         // Height, in pixels, of the image
    BYTE        bColorCount;     // Number of colors in image (0 if >=8bpp)
    BYTE        bReserved;       // Reserved ( must be 0)
    WORD        wPlanes;         // Color Planes
    WORD        wBitCount;       // Bits per pixel
    DWORD       dwBytesInRes;    // How many bytes in this resource?
    DWORD       dwImageOffset;   // Where in the file is this image?
} ICONDIRENTRY, *PICONDIRENTRY;

// main structure of .ico file
typedef struct
{
    WORD           idReserved;   // Reserved (must be 0)
    WORD           idType;       // Resource Type (1 for icons)
    WORD           idCount;      // How many images?
    ICONDIRENTRY   idEntries[1]; // An entry for each image (idCount of 'em)
} ICONDIR, *PICONDIR;

///////////////////////////////////////////////////////////////////////////////////

// icon structures used in .dll + .exe files
// RT_GROPU

// note: Use nID menber with FindResource, LoadResource and LockResource to
// get a pointer to an ICONIMAGE object (struct is defined above)
typedef struct
{
  BYTE   bWidth;               // Width, in pixels, of the image
  BYTE   bHeight;              // Height, in pixels, of the image
  BYTE   bColorCount;          // Number of colors in image (0 if >=8bpp)
  BYTE   bReserved;            // Reserved
  WORD   wPlanes;              // Color Planes
  WORD   wBitCount;            // Bits per pixel
  DWORD  dwBytesInRes;         // how many bytes in this resource?
  WORD   nID;                  // the ID
} GRPICONDIRENTRY, *PGRPICONDIRENTRY;


// RT_GROUP_ICON resource
typedef struct 
{
  WORD            idReserved;   // Reserved (must be 0)
  WORD            idType;       // Resource type (1 for icons)
  WORD            idCount;      // How many images?
  GRPICONDIRENTRY idEntries[1]; // The entries for each image
} GRPICONDIR, *PGRPICONDIR;


#pragma pack(pop)


