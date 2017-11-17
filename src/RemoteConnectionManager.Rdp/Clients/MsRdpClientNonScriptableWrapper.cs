using System;
using System.Runtime.InteropServices;

namespace RemoteConnectionManager.Rdp.Clients
{
    // Source: https://social.msdn.microsoft.com/Forums/windowsdesktop/en-US/9095625c-4361-4e0b-bfcf-be15550b60a8/imsrdpclientnonscriptablesendkeys?forum=windowsgeneraldevelopmentissues
    internal class MsRdpClientNonScriptableWrapper
    {
        [InterfaceType(1)]
        [Guid("2F079C4C-87B2-4AFD-97AB-20CDB43038AE")]
        private interface IMsRdpClientNonScriptable_Sendkeys : MSTSCLib.IMsTscNonScriptable
        {
            [DispId(4)]
            string BinaryPassword { get; set; }
            [DispId(5)]
            string BinarySalt { get; set; }
            [DispId(1)]
            string ClearTextPassword { set; }
            [DispId(2)]
            string PortablePassword { get; set; }
            [DispId(3)]
            string PortableSalt { get; set; }

            void NotifyRedirectDeviceChange(uint wParam, int lParam);
            void ResetPassword();

            unsafe void SendKeys(int numKeys, int* pbArrayKeyUp, int* plKeyData);
        }

        private IMsRdpClientNonScriptable_Sendkeys m_ComInterface;

        public MsRdpClientNonScriptableWrapper(object ocx)
        {
            m_ComInterface = (IMsRdpClientNonScriptable_Sendkeys)ocx;
        }

        public void SendKeys(int[] keyScanCodes, bool[] keyReleased)
        {
            if (keyScanCodes.Length != keyReleased.Length) throw new ArgumentException("MsRdpClientNonScriptableWrapper.SendKeys: Arraysize must match");
            
            int[] temp = new int[keyReleased.Length];
            for (int i = 0; i < temp.Length; i++) temp[i] = keyReleased[i] ? 1 : 0;

            unsafe
            {
                fixed (int* pScanCodes = keyScanCodes)
                fixed (int* pKeyReleased = temp)
                {
                    m_ComInterface.SendKeys(keyScanCodes.Length, pKeyReleased, pScanCodes);
                }
            }
        }
    }
}
