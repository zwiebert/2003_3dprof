int
setclk_open()
{
  int result = 0;
  BIOSPLLINFO bioshdr;
  DWORD tmp;
  if (!r6probe_open()) {
    return = result;
  }
  DWORD bh = r6probe_readbios(R6_BIOS_HEADER_LOC);
  bh += 0x30; // offset to pll info ptr
  r6probe_readbios(bh);

  
}
