﻿using Microsoft.VisualStudio.OLE.Interop;
using System;
using IS = System.Runtime.InteropServices;

namespace Trebuchet
{   
    [IS.ComVisible(true)]
    public class TestClass : IMarshal, IStorage
    {
        private IStorage _stg;
        private string _binding;

        public TestClass(IStorage stg, string binding)
        {
            _stg = stg;
            _binding = binding;
        }

        public void DisconnectObject(uint dwReserved)
        {
        }

        public void GetMarshalSizeMax(ref Guid riid, IntPtr pv, uint dwDestContext, IntPtr pvDestContext, uint MSHLFLAGS, out uint pSize)
        {
            pSize = 1024;
        }

        public void GetUnmarshalClass(ref Guid riid, IntPtr pv, uint dwDestContext, IntPtr pvDestContext, uint MSHLFLAGS, out Guid pCid)
        {
            pCid = new Guid("00000306-0000-0000-c000-000000000046");
        }

        public void MarshalInterface(IStream pstm, ref Guid riid, IntPtr pv, uint dwDestContext, IntPtr pvDestContext, uint MSHLFLAGS)
        {
            uint written;
            byte[] data = ComUtils.CreateStandardMarshal(_binding);

            pstm.Write(data, (uint)data.Length, out written);
        }

        public void ReleaseMarshalData(IStream pstm)
        {
        }

        public void UnmarshalInterface(IStream pstm, ref Guid riid, out IntPtr ppv)
        {
            ppv = IntPtr.Zero;
        }

        public void Commit(uint grfCommitFlags)
        {
            _stg.Commit(grfCommitFlags);
        }

        public void CopyTo(uint ciidExclude, Guid[] rgiidExclude, IntPtr snbExclude, IStorage pstgDest)
        {
            _stg.CopyTo(ciidExclude, rgiidExclude, snbExclude, pstgDest);
        }

        public void CreateStorage(string pwcsName, uint grfMode, uint reserved1, uint reserved2, out IStorage ppstg)
        {
            _stg.CreateStorage(pwcsName, grfMode, reserved1, reserved2, out ppstg);
        }

        public void CreateStream(string pwcsName, uint grfMode, uint reserved1, uint reserved2, out IStream ppstm)
        {
            _stg.CreateStream(pwcsName, grfMode, reserved1, reserved2, out ppstm);
        }

        public void DestroyElement(string pwcsName)
        {
            _stg.DestroyElement(pwcsName);
        }

        public void EnumElements(uint reserved1, IntPtr reserved2, uint reserved3, out IEnumSTATSTG ppEnum)
        {
            _stg.EnumElements(reserved1, reserved2, reserved3, out ppEnum);
        }

        public void MoveElementTo(string pwcsName, IStorage pstgDest, string pwcsNewName, uint grfFlags)
        {
            _stg.MoveElementTo(pwcsName, pstgDest, pwcsNewName, grfFlags);
        }

        public void OpenStorage(string pwcsName, IStorage pstgPriority, uint grfMode, IntPtr snbExclude, uint reserved, out IStorage ppstg)
        {
            _stg.OpenStorage(pwcsName, pstgPriority, grfMode, snbExclude, reserved, out ppstg);
        }

        public void OpenStream(string pwcsName, IntPtr reserved1, uint grfMode, uint reserved2, out IStream ppstm)
        {
            _stg.OpenStream(pwcsName, reserved1, grfMode, reserved2, out ppstm);
        }

        public void RenameElement(string pwcsOldName, string pwcsNewName)
        {

        }

        public void Revert()
        {

        }

        public void SetClass(ref Guid clsid)
        {

        }

        public void SetElementTimes(string pwcsName, FILETIME[] pctime, FILETIME[] patime, FILETIME[] pmtime)
        {

        }

        public void SetStateBits(uint grfStateBits, uint grfMask)
        {
        }

        public void Stat(STATSTG[] pstatstg, uint grfStatFlag)
        {
            _stg.Stat(pstatstg, grfStatFlag);
            pstatstg[0].pwcsName = "hello.stg";
        }    
    }
}