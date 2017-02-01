=============================================================================
       CLASS LIBRARY : CSShellExtContextMenuHandler Project Overview
=============================================================================

/////////////////////////////////////////////////////////////////////////////
Summary:

The C# code sample demonstrates creating a Shell context menu handler with 
.NET Framework 4.

A context menu handler is a shell extension handler that adds commands to an 
existing context menu. Context menu handlers are associated with a particular 
file class and are called any time a context menu is displayed for a member 
of the class. While you can add items to a file class context menu with the 
registry, the items will be the same for all members of the class. By 
implementing and registering such a handler, you can dynamically add items to 
an object's context menu, customized for the particular object.

Context menu handler is the most powerful but also the most complicated method 
to implement. It is strongly encouraged that you implement a context menu 
using one of the static verb methods if applicable:
http://msdn.microsoft.com/en-us/library/dd758091.aspx

Prior to .NET Framework 4, the development of in-process shell extensions 
using managed code is not officially supported because of the CLR limitation 
allowing only one .NET runtime per process. Jesse Kaplan, one of the CLR 
program managers, explains it in this MSDN forum thread: 
http://social.msdn.microsoft.com/forums/en-US/netfxbcl/thread/1428326d-7950-42b4-ad94-8e962124043e.

In .NET 4, with the ability to have multiple runtimes in process with any 
other runtime, Microsoft can now offer general support for writing managed 
shell extensions—even those that run in-process with arbitrary applications 
on the machine. CSShellExtContextMenuHandler is such a managed shell extension 
code example. However, please note that you still cannot write shell 
extensions using any version earlier than .NET Framework 4 because those 
versions of the runtime do not load in-process with one another and will 
cause failures in many cases.

The example context menu handler has the class ID (CLSID): 
    {B1F1405D-94A1-4692-B72F-FC8CAF8B8700}

It adds the menu item "Display File Name (C#)" with icon to the context menu 
when you right-click a .cs file in the Windows Explorer. Clicking the menu 
item brings up a message box that displays the full path of the .cs file.


/////////////////////////////////////////////////////////////////////////////
Setup and Removal:

--------------------------------------
In the Development Environment

A. Setup

Run 'Visual Studio Command Prompt (2010)' (or 'Visual Studio x64 Win64 
Command Prompt (2010)' if you are on a x64 operating system) in the Microsoft 
Visual Studio 2010 \ Visual Studio Tools menu as administrator. Navigate to 
the folder that contains the build result CSShellExtContextMenuHandler.dll 
and enter the command:

    Regasm.exe CSShellExtContextMenuHandler.dll /codebase

The context menu handler is registered successfully if the command prints:

    "Types registered successfully"

B. Removal

Run 'Visual Studio Command Prompt (2010)' (or 'Visual Studio x64 Win64 
Command Prompt (2010)' if you are on a x64 operating system) in the Microsoft 
Visual Studio 2010 \ Visual Studio Tools menu as administrator. Navigate to 
the folder that contains the build result CSShellExtContextMenuHandler.dll 
and enter the command:

    Regasm.exe CSShellExtContextMenuHandler.dll /unregister

The context menu handler is unregistered successfully if the command prints:

    "Types un-registered successfully"

--------------------------------------
In the Deployment Environment

A. Setup

Install CSShellExtContextMenuHandlerSetup(x86).msi, the output of the 
CSShellExtContextMenuHandlerSetup(x86) setup project, on a x86 operating 
system. If the target operating system is x64, install 
CSShellExtContextMenuHandlerSetup(x64).msi outputted by the 
CSShellExtContextMenuHandlerSetup(x64) setup project.

B. Removal

Uninstall CSShellExtContextMenuHandlerSetup(x86).msi, the output of the 
CSShellExtContextMenuHandlerSetup(x86) setup project, on a x86 operating 
system. If the target operating system is x64, uninstall 
CSShellExtContextMenuHandlerSetup(x64).msi outputted by the 
CSShellExtContextMenuHandlerSetup(x64) setup project.


/////////////////////////////////////////////////////////////////////////////
Demo:

The following steps walk through a demonstration of the context menu handler 
code sample.

Step1. After you successfully build the sample project in Visual Studio 2010, 
you will get a DLL: CSShellExtContextMenuHandler.dll. Run 'Visual Studio 
Command Prompt (2010)' (or 'Visual Studio x64 Win64 Command Prompt (2010)' if 
you are on a x64 operating system) in the Microsoft Visual Studio 2010 \ 
Visual Studio Tools menu as administrator. Navigate to the folder that 
contains the build result CSShellExtContextMenuHandler.dll and enter the 
command:

    Regasm.exe CSShellExtContextMenuHandler.dll /codebase

The context menu handler is registered successfully if the command prints:

    "Types registered successfully"

Step2. Find a .cs file in the Windows Explorer (e.g. FileContextMenuExt.cs in 
the sample folder), and right click it. You would see the "Display File Name 
(C#)" menu item with icon in the context menu and a menu seperator below it. 
Clicking the menu item brings up a message box that displays the full path of 
the .cs file.

The "Display File Name (C#)" menu item is added and displayed when only one 
.cs file is selected and right-clicked. If more than one file are selected 
in the Windows Explorer, you will not see the context menu item.

Step3. In the same Visual Studio command prompt, run the command 

    Regasm.exe CSShellExtContextMenuHandler.dll /unregister

to unregister the Shell context menu handler.


/////////////////////////////////////////////////////////////////////////////
Implementation:

A. Creating and configuring the project

In Visual Studio 2010, create a Visual C# / Windows / Class Library project 
named "CSShellExtContextMenuHandler". Open the project properties, and in the 
Signing page, sign the assembly with a strong name key file. 

-----------------------------------------------------------------------------

B. Implementing a basic Component Object Model (COM) DLL

Shell extension handlers are all in-process COM objects implemented as DLLs. 
Making a basic .NET COM component is very straightforward. You just need to 
define a 'public' class with ComVisible(true), use the Guid attribute to 
specify its CLSID, and explicitly implements certain COM interfaces. For 
example, 

    [ClassInterface(ClassInterfaceType.None)]
    [Guid("B1F1405D-94A1-4692-B72F-FC8CAF8B8700"), ComVisible(true)]
    public class SimpleObject : ISimpleObject
    {
        ... // Implements the interface
    }

You even do not need to implement IUnknown and class factory by yourself 
because .NET Framework handles them for you.

-----------------------------------------------------------------------------

C. Implementing the context menu handler and registering it for a certain 
file class

-----------
Implementing the context menu handler:

The FileContextMenuExt.cs file defines a context menu handler. The context 
menu handler must implement the IShellExtInit and IContextMenu interfaces. 
The interfaces are imported using the COMImport attribute in ShellExtLib.cs. 

    [ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("000214e8-0000-0000-c000-000000000046")]
    internal interface IShellExtInit
    {
        void Initialize(
            IntPtr pidlFolder,
            IntPtr pDataObj,
            IntPtr /*HKEY*/ hKeyProgID);
    }

    [ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("000214e4-0000-0000-c000-000000000046")]
    internal interface IContextMenu
    {
        [PreserveSig]
        int QueryContextMenu(
            IntPtr /*HMENU*/ hMenu,
            uint iMenu,
            uint idCmdFirst,
            uint idCmdLast,
            uint uFlags);

        void InvokeCommand(IntPtr pici);

        void GetCommandString(
            UIntPtr idCmd,
            uint uFlags,
            IntPtr pReserved,
            StringBuilder pszName,
            uint cchMax);
    }

    [ClassInterface(ClassInterfaceType.None)]
    [Guid("B1F1405D-94A1-4692-B72F-FC8CAF8B8700"), ComVisible(true)]
    public class FileContextMenuExt : IShellExtInit, IContextMenu
    {
        public void Initialize(IntPtr pidlFolder, IntPtr pDataObj, IntPtr hKeyProgID)
        {
            ...
        }
	
        public int QueryContextMenu(
            IntPtr hMenu,
            uint iMenu,
            uint idCmdFirst,
            uint idCmdLast,
            uint uFlags)
        {
            ...
        }

        public void InvokeCommand(IntPtr pici)
        {
            ...
        }

        public void GetCommandString(
            UIntPtr idCmd,
            uint uFlags,
            IntPtr pReserved,
            StringBuilder pszName,
            uint cchMax)
        {
            ...
        }
    }

When you write your own handler, you must create a new CLSID by using the 
"Create GUID" tool in the Tools menu for the shell extension class, and 
specify the value in the Guid attribute. 

    ...
    [Guid("B1F1405D-94A1-4692-B72F-FC8CAF8B8700"), ComVisible(true)]
    public class FileContextMenuExt : ...

The PreserveSig attribute indicates that the HRESULT or retval signature 
transformation that takes place during COM interop calls should be suppressed.
When you do not apply PreserveSigAttribute (e.g. the GetCommandString method 
of IContextMenu), the failure HRESULT of the method needs to be thrown as a 
.NET exception. For example, Marshal.ThrowExceptionForHR(WinError.E_FAIL); 
When you apply the PreserveSigAttribute to a managed method signature, the 
managed and unmanaged signatures of the attributed method are identical (e.g. 
the QueryContextMenu method of IContextMenu). Preserving the original method 
signature is necessary if the member returns more than one success HRESULT 
value and you want to detect the different values.

A context menu extension is instantiated when the user displays the context 
menu for an object of a class for which the context menu handler has been 
registered.

  1 Implementing IShellExtInit

  After the context menu extension COM object is instantiated, the 
  IShellExtInit.Initialize method is called. IShellExtInit.Initialize 
  supplies the context menu extension with an IDataObject object that 
  holds one or more file names in CF_HDROP format. You can enumerate the 
  selected files and folders through the IDataObject object. If a failure 
  HRESULT is returned (thrown) from IShellExtInit.Initialize, the context 
  menu extension will not be used.

  In the code sample, the FileContextMenuExt.Initialize method enumerates 
  the selected files and folders. If only one file is selected, the method 
  stores the file name for later use. If more than one file or no file are 
  selected, the method throws an exception with the E_FAIL HRESULT to not use 
  the context menu extension.

  2. Implementing IContextMenu

  After IShellExtInit.Initialize returns successfully, the 
  IContextMenu.QueryContextMenu method is called to obtain the menu item or 
  items that the context menu extension will add. The QueryContextMenu 
  implementation is fairly straightforward. The context menu extension adds 
  its menu items using the InsertMenuItem or similar function. The menu 
  command identifiers must be greater than or equal to idCmdFirst and must be 
  less than idCmdLast. QueryContextMenu must return the greatest numeric 
  identifier added to the menu plus one. The best way to assign menu command 
  identifiers is to start at zero and work up in sequence. If the context 
  menu extension does not need to add any items to the menu, it should simply 
  return from QueryContextMenu.

  In this code sample, we insert the menu item "Display File Name (C#)" with 
  an icon, and add a menu seperator below it.

  IContextMenu.GetCommandString is called to retrieve textual data for the 
  menu item, such as help text to be displayed for the menu item. If a user 
  highlights one of the items added by the context menu handler, the handler's 
  IContextMenu.GetCommandString method is called to request a Help text 
  string that will be displayed on the Windows Explorer status bar. This 
  method can also be called to request the verb string that is assigned to a 
  command. Either ANSI or Unicode verb strings can be requested. This example 
  only implements support for the Unicode values of uFlags, because only 
  those have been used in Windows Explorer since Windows 2000.

  IContextMenu.InvokeCommand is called when one of the menu items installed 
  by the context menu extension is selected. The context menu performs or 
  initiates the desired actions in response to this method.

-----------
Registering the handler for a certain file class:

Context menu handlers are associated with either a file class or a folder. 
For file classes, the handler is registered under the following subkey.

    HKEY_CLASSES_ROOT\<File Type>\shellex\ContextMenuHandlers

The registration of the context menu handler is implemented in the Register 
method of FileContextMenuExt. The ComRegisterFunction attribute attached to 
the method enables the execution of user-written code other than the basic 
registration of the COM class. Register calls the 
ShellExtReg.RegisterShellExtContextMenuHandler method in ShellExtLib.cs to 
associate the handler with a certain file type. If the file type starts with 
'.', it tries to read the default value of the HKCR\<File Type> key which may 
contain the Program ID to which the file type is linked. If the default value 
is not empty, use the Program ID as the file type to proceed the registration. 

For example, this code sample associates the handler with '.cs' files. 
HKCR\.cs has the default value 'VisualStudio.cs.10.0' by default when 
Visual Studio 2010 is installed, so we proceed to register the handler under 
HKCR\VisualStudio.cs.10.0\ instead of under HKCR\.cs. The following keys 
and values are added in the registration process of the sample handler. 

    HKCR
    {
        NoRemove .cs = s 'VisualStudio.cs.10.0'
        NoRemove VisualStudio.cs.10.0
        {
            NoRemove shellex
            {
                NoRemove ContextMenuHandlers
                {
                    {B1F1405D-94A1-4692-B72F-FC8CAF8B8700} = 
                        s 'CSShellExtContextMenuHandler.FileContextMenuExt Class'
                }
            }
        }
    }

The unregistration is implemented in the Unregister method of 
FileContextMenuExt. Similar to the Register method, the ComUnregisterFunction 
attribute attached to the method enables the execution of user-written code 
during the unregistration process. It removes the {<CLSID>} key under 
HKCR\<File Type>\shellex\ContextMenuHandlers.

-----------------------------------------------------------------------------

D. Deploying the context menu handler with a setup project.

  (1) In CSShellExtContextMenuHandler, add an Installer class (named 
  ProjectInstaller in this code sample) to define the custom actions in the 
  setup. The class derives from System.Configuration.Install.Installer. We 
  use the custom actions to register/unregister the COM-visible classes in 
  the current managed assembly when user installs/uninstalls the component. 

    [RunInstaller(true), ComVisible(false)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);

            // Call RegistrationServices.RegisterAssembly to register the classes in 
            // the current managed assembly to enable creation from COM.
            RegistrationServices regService = new RegistrationServices();
            regService.RegisterAssembly(
                this.GetType().Assembly, 
                AssemblyRegistrationFlags.SetCodeBase);
        }

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);

            // Call RegistrationServices.UnregisterAssembly to unregister the classes 
            // in the current managed assembly.
            RegistrationServices regService = new RegistrationServices();
            regService.UnregisterAssembly(this.GetType().Assembly);
        }
    }

  In the overriden Install method, we use RegistrationServices.RegisterAssembly 
  to register the classes in the current managed assembly to enable creation 
  from COM. The method also invokes the static method marked with 
  ComRegisterFunctionAttribute to perform the custom COM registration steps. 
  The call is equivalent to running the command: 
  
    "Regasm.exe CSShellExtContextMenuHandler.dll /codebase"

  in the development environment. 

  (2) To add a deployment project that deploys the Shell extension handler to 
  a x86 operating system, on the File menu, point to Add, and then click New 
  Project. In the Add New Project dialog box, expand the Other Project Types 
  node, expand the Setup and Deployment Projects, click Visual Studio 
  Installer, and then click Setup Project. In the Name box, type 
  CSShellExtContextMenuHandlerSetup(x86). Click OK to create the project. 
  In the Properties dialog of the setup project, make sure that the 
  TargetPlatform property is set to x86. This setup project is to be deployed 
  on x86 Windows operating systems. 

  Right-click the setup project, and choose Add / Project Output ... 
  In the Add Project Output Group dialog box, CSShellExtContextMenuHandler 
  will be displayed in the Project list. Select Primary Output and click OK.

  Right-click the setup project again, and choose View / Custom Actions. 
  In the Custom Actions Editor, right-click the root Custom Actions node. On 
  the Action menu, click Add Custom Action. In the Select Item in Project 
  dialog box, double-click the Application Folder. Select Primary output from 
  CSShellExtContextMenuHandler. This adds the custom actions that we defined 
  in ProjectInstaller of CSShellExtContextMenuHandler. 

  Right-click the setup project and select Properties. Click the 
  Prerequisites... button. In the Prerequisites dialog box, uncheck the 
  Microsoft .NET Framework 4 Client Profile (x86 and x64) option, and check 
  the Microsoft .NET Framework 4 (x86 and x64) option to match the .NET 
  Framework version of CSShellExtContextMenuHandler. 

  Build the setup project. If the build succeeds, you will get a .msi file 
  and a Setup.exe file. You can distribute them to your users to install or 
  uninstall your Shell extension handler. 

  (3) To deploy the Shell extension handler to a x64 operating system, you 
  must create a new setup project (e.g. CSShellExtContextMenuHandlerSetup(x64) 
  in this code sample), and set its TargetPlatform property to x64. 

  Although the TargetPlatform property is set to x64, the native shim 
  packaged with the .msi file is still a 32-bit executable. The Visual Studio 
  embeds the 32-bit version of InstallUtilLib.dll into the Binary table as 
  InstallUtil. So the custom actions will be run in the 32-bit, which is 
  unexpected in this code sample. To workaround this issue and ensure that 
  the custom actions run in the 64-bit mode, you either need to import the 
  appropriate bitness of InstallUtilLib.dll into the Binary table for the 
  InstallUtil record or - if you do have or will have 32-bit managed custom 
  actions add it as a new record in the Binary table and adjust the 
  CustomAction table to use the 64-bit Binary table record for 64-bit managed 
  custom actions. This blog article introduces how to do it manually with 
  Orca http://blogs.msdn.com/b/heaths/archive/2006/02/01/64-bit-managed-custom-actions-with-visual-studio.aspx

  In this code sample, we automate the modification of InstallUtil by using a 
  post-build javascript: Fix64bitInstallUtilLib.js. You can find the script 
  file in the CSShellExtContextMenuHandlerSetup(x64) project folder. To 
  configure the script to run in the post-build event, you select the 
  CSShellExtContextMenuHandlerSetup(x64) project in Solution Explorer, and 
  find the PostBuildEvent property in the Properties window. Specify its 
  value to be 
  
	"$(ProjectDir)Fix64bitInstallUtilLib.js" "$(BuiltOuputPath)" "$(ProjectDir)"

  Repeat the rest steps in (2) to add the project output, set the custom 
  actions, configure the prerequisites, and build the setup project.


/////////////////////////////////////////////////////////////////////////////
References:

MSDN: Initializing Shell Extensions
http://msdn.microsoft.com/en-us/library/cc144105.aspx

MSDN: Creating Context Menu Handlers
http://msdn.microsoft.com/en-us/library/bb776881.aspx

MSDN: Implementing the Context Menu COM Object
http://msdn.microsoft.com/en-us/library/ms677106.aspx

MSDN: Extending Shortcut Menus
http://msdn.microsoft.com/en-us/library/cc144101.aspx

MSDN: Choosing a Static or Dynamic Shortcut Menu Method
http://msdn.microsoft.com/en-us/library/dd758091.aspx

CLR Inside Out: In-Process Side-by-Side
http://msdn.microsoft.com/en-us/magazine/ee819091.aspx

Do not write in-process shell extensions in managed code before .NET 4
http://blogs.msdn.com/b/oldnewthing/archive/2006/12/18/1317290.aspx
http://social.msdn.microsoft.com/forums/en-US/netfxbcl/thread/1428326d-7950-42b4-ad94-8e962124043e/
http://blogs.msdn.com/b/junfeng/archive/2005/11/18/494572.aspx


/////////////////////////////////////////////////////////////////////////////