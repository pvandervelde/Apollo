﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Apollo.Core.Base.Activation
{
    /// <summary>
    /// Defines a 'job' that links all the active dataset processes to the
    /// current process so that all the processes may be terminated at the same
    /// time.
    /// </summary>
    internal class DatasetTrackingJob : IDisposable
    {
        private enum JobObjectInfoType
        {
            AssociateCompletionPortInformation = 7,
            BasicLimitInformation = 2,
            BasicUiRestrictions = 4,
            EndOfJobTimeInformation = 6,
            ExtendedLimitInformation = 9,
            SecurityLimitInformation = 5,
            GroupInformation = 11
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SecurityAttributes
        {
            public int Length;
            public IntPtr SecurityDescriptor;
            public int InheritHandle;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct JobObjectBasicLimitInformation
        {
            public long PerProcessUserTimeLimit;
            public long PerJobUserTimeLimit;
            public short LimitFlags;
            public UIntPtr MinimumWorkingSetSize;
            public UIntPtr MaximumWorkingSetSize;
            public short ActiveProcessLimit;
            public long Affinity;
            public short PriorityClass;
            public short SchedulingClass;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct IoCounters
        {
            public ulong ReadOperationCount;
            public ulong WriteOperationCount;
            public ulong OtherOperationCount;
            public ulong ReadTransferCount;
            public ulong WriteTransferCount;
            public ulong OtherTransferCount;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct JobObjectExtendedLimitInformation
        {
            public JobObjectBasicLimitInformation BasicLimitInformation;
            public IoCounters IoInfo;
            public UIntPtr ProcessMemoryLimit;
            public UIntPtr JobMemoryLimit;
            public UIntPtr PeakProcessMemoryUsed;
            public UIntPtr PeakJobMemoryUsed;
        }

        /// <summary>
        /// Defines the constant that indicates that child processes need to be killed
        /// when the main job is terminated.
        /// </summary>
        private const short JobObjectLimitKillOnJobClose = 0x00002000;

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass",
            Justification = "That just seems overkill. These methods belong here.")]
        private static extern IntPtr CreateJobObject(
            IntPtr jobAttributes,
            string name);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        [SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass",
            Justification = "That just seems overkill. These methods belong here.")]
        private static extern bool SetInformationJobObject(
            IntPtr hJob,
            JobObjectInfoType infoType,
            IntPtr lpJobObjectInfo,
            uint cbJobObjectInfoLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        [SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass",
            Justification = "That just seems overkill. These methods belong here.")]
        private static extern bool AssignProcessToJobObject(IntPtr job, IntPtr process);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        [SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass",
            Justification = "That just seems overkill. These methods belong here.")]
        private static extern bool CloseHandle(IntPtr hObject);

        /// <summary>
        /// The handle for the job that links all the child processes to the current
        /// process.
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources",
            Justification = "To be implemented.")]
        private IntPtr m_Handle;

        /// <summary>
        /// The flag that indicates if the object has been disposed or not.
        /// </summary>
        private volatile bool m_IsDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetTrackingJob"/> class.
        /// </summary>
        public DatasetTrackingJob()
        {
            m_Handle = CreateJobObject(IntPtr.Zero, "Apollo");
            if (m_Handle == IntPtr.Zero)
            {
                var error = Marshal.GetLastWin32Error();
                throw new UnableToCreateJobException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Unable to create job object.  Error: {0}",
                        error));
            }

            var info = new JobObjectBasicLimitInformation
                {
                    LimitFlags = JobObjectLimitKillOnJobClose
                };

            var extendedInfo = new JobObjectExtendedLimitInformation
                {
                    BasicLimitInformation = info
                };

            int length = Marshal.SizeOf(typeof(JobObjectExtendedLimitInformation));
            var extendedInfoPtr = Marshal.AllocHGlobal(length);
            Marshal.StructureToPtr(extendedInfo, extendedInfoPtr, false);

            if (!SetInformationJobObject(
                m_Handle,
                JobObjectInfoType.ExtendedLimitInformation,
                extendedInfoPtr,
                (uint)length))
            {
                var error = Marshal.GetLastWin32Error();
                throw new UnableToSetJobException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Unable to set information.  Error: {0}",
                        error));
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="DatasetTrackingJob"/> class.
        /// </summary>
        ~DatasetTrackingJob()
        {
            CleanUpResources();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            CleanUpResources();
            GC.SuppressFinalize(this);
        }

        private void CleanUpResources()
        {
            if (m_IsDisposed)
            {
                return;
            }

            Close();
            m_IsDisposed = true;
        }

        /// <summary>
        /// Closes the job.
        /// </summary>
        private void Close()
        {
            if (!CloseHandle(m_Handle))
            {
            }

            m_Handle = IntPtr.Zero;
        }

        /// <summary>
        /// Links a new child process to the current job.
        /// </summary>
        /// <param name="processToAdd">The child process that should be added.</param>
        public void LinkChildProcessToJob(Process processToAdd)
        {
            if (!AssignProcessToJobObject(m_Handle, processToAdd.Handle))
            {
                var error = Marshal.GetLastWin32Error();
                throw new UnableToLinkChildProcessToJobException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Unable to link child process to Job. Error: {0}",
                        error));
            }
        }
    }
}
