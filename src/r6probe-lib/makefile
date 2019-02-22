!include ..\master.mk

all:
    cd sys
    $(MAKE) $(MKFLAGS)
    cd ..\vxd
    $(MAKE) $(MKFLAGS)
    cd ..\src
    $(MAKE) $(MKFLAGS)    

clean:
    cd sys
    $(MAKE) $(MKFLAGS) clean
    cd ..\vxd
    $(MAKE) $(MKFLAGS) clean
    cd ..\src
    $(MAKE) $(MKFLAGS) clean
