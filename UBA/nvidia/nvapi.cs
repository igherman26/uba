using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OS_module_cs.nvidia
{
    class NVAPI
    {
        internal enum NvStatus
        {
            OK = 0,
            ERROR = -1,
            LIBRARY_NOT_FOUND = -2,
            NO_IMPLEMENTATION = -3,
            API_NOT_INTIALIZED = -4,
            INVALID_ARGUMENT = -5,
            NVIDIA_DEVICE_NOT_FOUND = -6,
            END_ENUMERATION = -7,
            INVALID_HANDLE = -8,
            INCOMPATIBLE_STRUCT_VERSION = -9,
            HANDLE_INVALIDATED = -10,
            OPENGL_CONTEXT_NOT_CURRENT = -11,
            NO_GL_EXPERT = -12,
            INSTRUMENTATION_DISABLED = -13,
            EXPECTED_LOGICAL_GPU_HANDLE = -100,
            EXPECTED_PHYSICAL_GPU_HANDLE = -101,
            EXPECTED_DISPLAY_HANDLE = -102,
            INVALID_COMBINATION = -103,
            NOT_SUPPORTED = -104,
            PORTID_NOT_FOUND = -105,
            EXPECTED_UNATTACHED_DISPLAY_HANDLE = -106,
            INVALID_PERF_LEVEL = -107,
            DEVICE_BUSY = -108,
            NV_PERSIST_FILE_NOT_FOUND = -109,
            PERSIST_DATA_NOT_FOUND = -110,
            EXPECTED_TV_DISPLAY = -111,
            EXPECTED_TV_DISPLAY_ON_DCONNECTOR = -112,
            NO_ACTIVE_SLI_TOPOLOGY = -113,
            SLI_RENDERING_MODE_NOTALLOWED = -114,
            EXPECTED_DIGITAL_FLAT_PANEL = -115,
            ARGUMENT_EXCEED_MAX_SIZE = -116,
            DEVICE_SWITCHING_NOT_ALLOWED = -117,
            TESTING_CLOCKS_NOT_SUPPORTED = -118,
            UNKNOWN_UNDERSCAN_CONFIG = -119,
            TIMEOUT_RECONFIGURING_GPU_TOPO = -120,
            DATA_NOT_FOUND = -121,
            EXPECTED_ANALOG_DISPLAY = -122,
            NO_VIDLINK = -123,
            REQUIRES_REBOOT = -124,
            INVALID_HYBRID_MODE = -125,
            MIXED_TARGET_TYPES = -126,
            SYSWOW64_NOT_SUPPORTED = -127,
            IMPLICIT_SET_GPU_TOPOLOGY_CHANGE_NOT_ALLOWED = -128,
            REQUEST_USER_TO_CLOSE_NON_MIGRATABLE_APPS = -129,
            OUT_OF_MEMORY = -130,
            WAS_STILL_DRAWING = -131,
            FILE_NOT_FOUND = -132,
            TOO_MANY_UNIQUE_STATE_OBJECTS = -133,
            INVALID_CALL = -134,
            D3D10_1_LIBRARY_NOT_FOUND = -135,
            FUNCTION_NOT_FOUND = -136
        }

        internal struct NvPhysicalGpuHandle
        {
            private readonly IntPtr ptr;
        }

        internal struct NvUsages
        {
            public uint Version;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 33)]
            public uint[] Usage;
        }

        private delegate IntPtr nvapi_QueryInterfaceDelegate(uint id);
        private delegate NvStatus NvAPI_InitializeDelegate();
        private delegate NvStatus NvAPI_GPU_GetFullNameDelegate(
          NvPhysicalGpuHandle gpuHandle, StringBuilder name);

        public delegate NvStatus NvAPI_EnumPhysicalGPUsDelegate(
     [Out] NvPhysicalGpuHandle[] gpuHandles, out int gpuCount);
        public delegate NvStatus NvAPI_GPU_GetUsagesDelegate(
      NvPhysicalGpuHandle gpuHandle, ref NvUsages nvUsages);

        private static readonly bool available;
        private static readonly nvapi_QueryInterfaceDelegate nvapi_QueryInterface;
        private static readonly NvAPI_InitializeDelegate NvAPI_Initialize;
        private static readonly NvAPI_GPU_GetFullNameDelegate
      _NvAPI_GPU_GetFullName;

        public static readonly NvAPI_EnumPhysicalGPUsDelegate
      NvAPI_EnumPhysicalGPUs;
        public static readonly NvAPI_GPU_GetUsagesDelegate
     NvAPI_GPU_GetUsages;

        public static NvStatus NvAPI_GPU_GetFullName(NvPhysicalGpuHandle gpuHandle,
      out string name)
        {
            StringBuilder builder = new StringBuilder(64);
            NvStatus status;
            if (_NvAPI_GPU_GetFullName != null)
                status = _NvAPI_GPU_GetFullName(gpuHandle, builder);
            else
                status = NvStatus.FUNCTION_NOT_FOUND;
            name = builder.ToString();
            return status;
        }

        private static string GetDllName()
        {
            if (IntPtr.Size == 4)
            {
                return "nvapi.dll";
            }
            else
            {
                return "nvapi64.dll";
            }
        }

        private static void GetDelegate<T>(uint id, out T newDelegate)
          where T : class
        {
            IntPtr ptr = nvapi_QueryInterface(id);
            if (ptr != IntPtr.Zero)
            {
                newDelegate =
                  Marshal.GetDelegateForFunctionPointer(ptr, typeof(T)) as T;
            }
            else
            {
                newDelegate = null;
            }
        }

        static NVAPI()
        {
            DllImportAttribute attribute = new DllImportAttribute(GetDllName());
            attribute.CallingConvention = CallingConvention.Cdecl;
            attribute.PreserveSig = true;
            attribute.EntryPoint = "nvapi_QueryInterface";
            PInvokeDelegateFactory.CreateDelegate(attribute,
              out nvapi_QueryInterface);

            try
            {
                GetDelegate(0x0150E828, out NvAPI_Initialize);
            }
            catch (DllNotFoundException) { return; }
            catch (EntryPointNotFoundException) { return; }
            catch (ArgumentNullException) { return; }

            if (NvAPI_Initialize() == NvStatus.OK)
            {
                GetDelegate(0xCEEE8E9F, out _NvAPI_GPU_GetFullName);
                GetDelegate(0xE5AC921F, out NvAPI_EnumPhysicalGPUs);
                GetDelegate(0x189A1FDF, out NvAPI_GPU_GetUsages);

                available = true;
            }
        }

        public static bool IsAvailable
        {
            get { return available; }
        }
    }
}
