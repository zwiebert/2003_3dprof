!include ..\master.mk

all:
    cd src
    $(MAKE) $(MKFLAGS)
    cd ..
    cd samples\setclk
    $(MAKE) $(MKFLAGS)
    cd ..\..
#    cd samples\dclkinfo
#    $(MAKE) $(MKFLAGS)
#    cd ..\..

clean:
    cd src
    $(MAKE) $(MKFLAGS) clean
    cd ..\samples\dclkinfo
    $(MAKE) $(MKFLAGS) clean
    cd ..\setclk
    $(MAKE) $(MKFLAGS) clean
