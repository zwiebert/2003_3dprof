
#define USE_SETUPAPI_DLL 0

#if USE_SETUPAPI_DLL

#ifdef __cplusplus
extern "C"
{
#endif


unsigned long get_mem_base(unsigned idx);
unsigned long get_mem_size(unsigned idx);
unsigned short get_device_id();

#ifdef __cplusplus
}
#endif

#endif /* USE_SETUPAPI_DLL */
