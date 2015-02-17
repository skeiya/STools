STools
======

An useful extension of Visual Studio.


buildtype := release
ifeq ($(buildtype), release)
else ifeq ($(buildtype), debug)
else
        $(erro buildtype mult be release, debug, profile or coverage)
endif

OUTDIR := Build/$(buildtype)
PROG := $(OUTDIR)/myapp

include FileList.mak
include CFileList.mak
OBJS := $(SRCS:%.cpp=$(OUTDIR)/%.o)
OBJS += $(CSRCS:%.c=$(OUTDIR)/%.o)
DEPS := $(SRCS:%.cpp=$(OUTDIR)/%.d)
DEPS += $(CSRCS:%.c=$(OUTDIR)/%.d)

CXX := g++
CC := gcc

all: $(PROG)

$(PROG): $(OBJS)
        $(CXX) -o $@ $^

$(OUTDIR)/%.o: %.cpp
        @if [ ! -e `dirname $@` ]; then mkdir -p `dirname $@`; fi
        $(CXX) -o $@ -c -MMD -MP -MF $(@:%.o=%.d) $<

$(OUTDIR)/%.o: %.c
        @if [ ! -e `dirname $@` ]; then mkdir -p `dirname $@`; fi
        $(CC) -c c++ -o $@ -c -MMD -MP -MF $(@:%.o=%.d) $<

-include $(DEPS)

clean:
        rm -rf $(OUTDIR)
