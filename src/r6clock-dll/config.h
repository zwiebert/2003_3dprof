
// choose backend/driver

#if !defined USE_DRV_R6P && !defined USE_DRV_WIO
  #define USE_DRV_WIO
  //#define USE_DRV_R6P
#endif

#if defined USE_DRV_R6P && defined USE_DRV_WIO
#error "USE_DRV_R6P and USE_DRV_WIO are used at the same time, which is not allowed!"
#endif