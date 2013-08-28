
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = dmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG;TRACE"
ASSEMBLY = bin/Debug/MonoMultiJack.OS.Windows.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../MonoMultiJack.Configuration/bin/Debug/MonoMultiJack.Configuration.dll \
	../MonoMultiJack.ConnectionWrapper.Jack/bin/Debug/MonoMultiJack.ConnectionWrapper.Jack.dll \
	../MonoMultiJack.ConnectionWrapper/bin/Debug/MonoMultiJack.ConnectionWrapper.dll \
	../MonoMultiJack.OS/bin/Debug/MonoMultiJack.OS.dll
BUILD_DIR = bin/Debug/

MONOMULTIJACK_OS_WINDOWS_DLL_MDB_SOURCE=bin/Debug/MonoMultiJack.OS.Windows.dll.mdb
MONOMULTIJACK_OS_WINDOWS_DLL_MDB=$(BUILD_DIR)/MonoMultiJack.OS.Windows.dll.mdb
MONOMULTIJACK_CONFIGURATION_DLL_SOURCE=../MonoMultiJack.Configuration/bin/Debug/MonoMultiJack.Configuration.dll
MONOMULTIJACK_CONFIGURATION_DLL_MDB_SOURCE=../MonoMultiJack.Configuration/bin/Debug/MonoMultiJack.Configuration.dll.mdb
MONOMULTIJACK_CONFIGURATION_DLL_MDB=$(BUILD_DIR)/MonoMultiJack.Configuration.dll.mdb
MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL_SOURCE=../MonoMultiJack.ConnectionWrapper.Jack/bin/Debug/MonoMultiJack.ConnectionWrapper.Jack.dll
MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL_CONFIG_SOURCE=../MonoMultiJack.ConnectionWrapper.Jack/app.config
MONOMULTIJACK_CONNECTIONWRAPPER_DLL_SOURCE=../MonoMultiJack.ConnectionWrapper/bin/Debug/MonoMultiJack.ConnectionWrapper.dll
MONOMULTIJACK_CONNECTIONWRAPPER_DLL_MDB_SOURCE=../MonoMultiJack.ConnectionWrapper/bin/Debug/MonoMultiJack.ConnectionWrapper.dll.mdb
MONOMULTIJACK_CONNECTIONWRAPPER_DLL_MDB=$(BUILD_DIR)/MonoMultiJack.ConnectionWrapper.dll.mdb
MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL_MDB_SOURCE=../MonoMultiJack.ConnectionWrapper.Jack/bin/Debug/MonoMultiJack.ConnectionWrapper.Jack.dll.mdb
MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL_MDB=$(BUILD_DIR)/MonoMultiJack.ConnectionWrapper.Jack.dll.mdb
MONOMULTIJACK_OS_DLL_SOURCE=../MonoMultiJack.OS/bin/Debug/MonoMultiJack.OS.dll
MONOMULTIJACK_OS_DLL_MDB_SOURCE=../MonoMultiJack.OS/bin/Debug/MonoMultiJack.OS.dll.mdb
MONOMULTIJACK_OS_DLL_MDB=$(BUILD_DIR)/MonoMultiJack.OS.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = dmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ "-define:TRACE"
ASSEMBLY = bin/Release/MonoMultiJack.OS.Windows.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../MonoMultiJack.Configuration/bin/Release/MonoMultiJack.Configuration.dll \
	../MonoMultiJack.ConnectionWrapper.Jack/bin/Release/MonoMultiJack.ConnectionWrapper.Jack.dll \
	../MonoMultiJack.ConnectionWrapper/bin/Release/MonoMultiJack.ConnectionWrapper.dll \
	../MonoMultiJack.OS/bin/Release/MonoMultiJack.OS.dll
BUILD_DIR = bin/Release/

MONOMULTIJACK_OS_WINDOWS_DLL_MDB=
MONOMULTIJACK_CONFIGURATION_DLL_SOURCE=../MonoMultiJack.Configuration/bin/Release/MonoMultiJack.Configuration.dll
MONOMULTIJACK_CONFIGURATION_DLL_MDB=
MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL_SOURCE=../MonoMultiJack.ConnectionWrapper.Jack/bin/Release/MonoMultiJack.ConnectionWrapper.Jack.dll
MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL_CONFIG_SOURCE=../MonoMultiJack.ConnectionWrapper.Jack/app.config
MONOMULTIJACK_CONNECTIONWRAPPER_DLL_SOURCE=../MonoMultiJack.ConnectionWrapper/bin/Release/MonoMultiJack.ConnectionWrapper.dll
MONOMULTIJACK_CONNECTIONWRAPPER_DLL_MDB=
MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL_MDB=
MONOMULTIJACK_OS_DLL_SOURCE=../MonoMultiJack.OS/bin/Release/MonoMultiJack.OS.dll
MONOMULTIJACK_OS_DLL_MDB=

endif

AL=al
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(MONOMULTIJACK_OS_WINDOWS_DLL_MDB) \
	$(MONOMULTIJACK_CONFIGURATION_DLL) \
	$(MONOMULTIJACK_CONFIGURATION_DLL_MDB) \
	$(MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL) \
	$(MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL_CONFIG) \
	$(MONOMULTIJACK_CONNECTIONWRAPPER_DLL) \
	$(MONOMULTIJACK_CONNECTIONWRAPPER_DLL_MDB) \
	$(MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL_MDB) \
	$(MONOMULTIJACK_OS_DLL) \
	$(MONOMULTIJACK_OS_DLL_MDB)  

LINUX_PKGCONFIG = \
	$(MONOMULTIJACK_OS_WINDOWS_PC)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

FILES = \
	ConnectionManagerFactory.cs \
	Program.cs \
	AssemblyInfo.cs 

DATA_FILES = 

RESOURCES = 

EXTRAS = \
	monomultijack.os.windows.pc.in 

REFERENCES =  \
	System \
	System.Core \
	System.Xml.Linq \
	System.Data.DataSetExtensions \
	Microsoft.CSharp \
	System.Data \
	System.Xml

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

include $(top_srcdir)/Makefile.include

MONOMULTIJACK_CONFIGURATION_DLL = $(BUILD_DIR)/MonoMultiJack.Configuration.dll
MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL = $(BUILD_DIR)/MonoMultiJack.ConnectionWrapper.Jack.dll
MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL_CONFIG = $(BUILD_DIR)/MonoMultiJack.ConnectionWrapper.Jack.dll.config
MONOMULTIJACK_CONNECTIONWRAPPER_DLL = $(BUILD_DIR)/MonoMultiJack.ConnectionWrapper.dll
MONOMULTIJACK_OS_DLL = $(BUILD_DIR)/MonoMultiJack.OS.dll
MONOMULTIJACK_OS_WINDOWS_PC = $(BUILD_DIR)/monomultijack.os.windows.pc

$(eval $(call emit-deploy-target,MONOMULTIJACK_CONFIGURATION_DLL))
$(eval $(call emit-deploy-target,MONOMULTIJACK_CONFIGURATION_DLL_MDB))
$(eval $(call emit-deploy-target,MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL))
$(eval $(call emit-deploy-target,MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL_CONFIG))
$(eval $(call emit-deploy-target,MONOMULTIJACK_CONNECTIONWRAPPER_DLL))
$(eval $(call emit-deploy-target,MONOMULTIJACK_CONNECTIONWRAPPER_DLL_MDB))
$(eval $(call emit-deploy-target,MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL_MDB))
$(eval $(call emit-deploy-target,MONOMULTIJACK_OS_DLL))
$(eval $(call emit-deploy-target,MONOMULTIJACK_OS_DLL_MDB))
$(eval $(call emit-deploy-wrapper,MONOMULTIJACK_OS_WINDOWS_PC,monomultijack.os.windows.pc))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY_MDB): $(ASSEMBLY)

$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
