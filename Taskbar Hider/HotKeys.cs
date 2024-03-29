﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Taskbar_Hider
{
    public class HotKeys
    {
        private readonly Dictionary<int, HotKeyCallBackHanlder> keymap = new Dictionary<int, HotKeyCallBackHanlder>();
        public delegate void HotKeyCallBackHanlder();

        public enum HotkeyModifiers
        {
            Alt = 1,
            Control = 2,
            Shift = 4,
            Win = 8
        }

        /// <summary>
        /// 热键消息
        /// </summary>
        public const int WM_HOTKEY = 0x312;

        /// <summary>
        /// 注册热键
        /// </summary>
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int modifiers, EKey vk);

        /// <summary>
        /// 注销热键
        /// </summary
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        /// <summary>
        /// 向原子表中添加全局原子
        /// </summary>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern short GlobalAddAtom(string lpString);

        /// <summary>
        /// 在表中搜索全局原子
        /// </summary>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern short GlobalFindAtom(string lpString);

        /// <summary>
        /// 在表中删除全局原子
        /// </summary>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern short GlobalDeleteAtom(string nAtom);

        /// <summary> 
        /// 注册快捷键
        /// </summary> 
        /// <param name="hWnd">持有快捷键窗口的句柄</param> 
        /// <param name="fsModifiers">组合键</param> 
        /// <param name="vk">快捷键的虚拟键码</param> 
        /// <param name="callBack">回调函数（按下快捷键时被调用的方法）</param> 
        public void Regist(IntPtr hWnd, int modifiers, EKey vk, HotKeyCallBackHanlder callBack)
        {
            int id = GlobalAddAtom("Taskbar Hider");
            if (!RegisterHotKey(hWnd, id, modifiers, vk))
                throw new Exception("注册失败！");
            keymap[id] = callBack;
        }

        /// <summary> 
        /// 注销快捷键
        /// </summary> 
        /// <param name="hWnd">持有快捷键窗口的句柄</param> 
        /// <param name="callBack">回调函数</param> 
        public void UnRegist(IntPtr hWnd, HotKeyCallBackHanlder callBack)
        {
            GlobalDeleteAtom("Taskbar Hider");
            foreach (KeyValuePair<int, HotKeyCallBackHanlder> var in keymap)
            {
                if (var.Value == callBack)
                    UnregisterHotKey(hWnd, var.Key);
            }
        }

        /// <summary>
        /// 窗体回调函数，接收所有窗体消息的事件处理函数
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="msg">消息</param>
        /// <param name="wideParam">附加参数1</param>
        /// <param name="longParam">附加参数2</param>
        /// <param name="handled">是否处理</param>
        /// <returns>返回句柄</returns>
        public IntPtr OnHotkey(IntPtr hWnd, int msg, IntPtr wideParam, IntPtr longParam, ref bool handled)
        {
            switch (msg)
            {
                case HotKeys.WM_HOTKEY:
                    int id = wideParam.ToInt32();
                    HotKeyCallBackHanlder? callback;
                    if (keymap.TryGetValue(id, out callback))
                        callback();
                    break;
                default:
                    break;
            }
            return IntPtr.Zero;
        }

        public enum EKey
        {
            //
            // 摘要:
            //     若要从一个密钥值中提取修饰符位掩码。
            //Modifiers = -65536,
            //
            // 摘要:
            //     不按任何键。
            None = 0,
            //
            // 摘要:
            //     鼠标左键。
            LButton = 1,
            //
            // 摘要:
            //     鼠标右键按钮中。
            RButton = 2,
            //
            // 摘要:
            //     CANCEL 键。
            Cancel = 3,
            //
            // 摘要:
            //     鼠标中键 （三个按钮的鼠标）。
            MButton = 4,
            //
            // 摘要:
            //     第一个 x 鼠标按钮 （五个按钮的鼠标）。
            XButton1 = 5,
            //
            // 摘要:
            //     第二个鼠标按钮 （五个按钮的鼠标） x。
            XButton2 = 6,
            //
            // 摘要:
            //     BACKSPACE 键。
            Back = 8,
            //
            // 摘要:
            //     TAB 键。
            Tab = 9,
            //
            // 摘要:
            //     LINEFEED 键。
            LineFeed = 10,
            //
            // 摘要:
            //     CLEAR 键。
            Clear = 12,
            //
            // 摘要:
            //     ENTER 键。
            Enter = 13,
            //
            // 摘要:
            //     RETURN 键。
            Return = 13,
            //
            // 摘要:
            //     SHIFT 键。
            ShiftKey = 16,
            //
            // 摘要:
            //     CTRL 键。
            ControlKey = 17,
            //
            // 摘要:
            //     ALT 键。
            Menu = 18,
            //
            // 摘要:
            //     PAUSE 键。
            Pause = 19,
            //
            // 摘要:
            //     CAPS LOCK 键。
            CapsLock = 20,
            //
            // 摘要:
            //     CAPS LOCK 键。
            Capital = 20,
            //
            // 摘要:
            //     IME Kana 模式键。
            KanaMode = 21,
            //
            // 摘要:
            //     IME Hanguel 模式键。 (保留为了兼容; 使用HangulMode)
            HanguelMode = 21,
            //
            // 摘要:
            //     IME Hangul 模式键。
            HangulMode = 21,
            //
            // 摘要:
            //     IME Junja 模式键。
            JunjaMode = 23,
            //
            // 摘要:
            //     IME 最终模式键。
            FinalMode = 24,
            //
            // 摘要:
            //     IME Hanja 模式键。
            HanjaMode = 25,
            //
            // 摘要:
            //     IME Kanji 模式键。
            KanjiMode = 25,
            //
            // 摘要:
            //     ESC 键。
            Escape = 27,
            //
            // 摘要:
            //     IME convert 键。
            IMEConvert = 28,
            //
            // 摘要:
            //     IME nonconvert 键。
            IMENonconvert = 29,
            //
            // 摘要:
            //     IME 接受密钥，替换System.Windows.Forms.Keys.IMEAceept。
            IMEAccept = 30,
            //
            // 摘要:
            //     IME 接受密钥。 已过时，请System.Windows.Forms.Keys.IMEAccept相反。
            IMEAceept = 30,
            //
            // 摘要:
            //     IME 模式更改密钥。
            IMEModeChange = 31,
            //
            // 摘要:
            //     SPACEBAR 键。
            Space = 32,
            //
            // 摘要:
            //     PAGE UP 键。
            //Prior = 33,
            //
            // 摘要:
            //     PAGE UP 键。
            PageUp = 33,
            //
            // 摘要:
            //     PAGE DOWN 键。
            PageDown = 34,
            //
            // 摘要:
            //     PAGE DOWN 键。
            //Next = 34,
            //
            // 摘要:
            //     END 键。
            End = 35,
            //
            // 摘要:
            //     HOME 键。
            Home = 36,
            //
            // 摘要:
            //     LEFT ARROW 键。
            Left = 37,
            //
            // 摘要:
            //     UP ARROW 键。
            Up = 38,
            //
            // 摘要:
            //     RIGHT ARROW 键。
            Right = 39,
            //
            // 摘要:
            //     DOWN ARROW 键。
            Down = 40,
            //
            // 摘要:
            //     SELECT 键。
            Select = 41,
            //
            // 摘要:
            //     PRINT 键。
            Print = 42,
            //
            // 摘要:
            //     EXECUTE 键。
            Execute = 43,
            //
            // 摘要:
            //     PRINT SCREEN 键。
            //Snapshot = 44,
            //
            // 摘要:
            //     PRINT SCREEN 键。
            PrintScreen = 44,
            //
            // 摘要:
            //     INS 键。
            Insert = 45,
            //
            // 摘要:
            //     DEL 键。
            Delete = 46,
            //
            // 摘要:
            //     HELP 键。
            Help = 47,
            //
            // 摘要:
            //     0 键。
            D0 = 48,
            //
            // 摘要:
            //     1 键。
            D1 = 49,
            //
            // 摘要:
            //     2 键。
            D2 = 50,
            //
            // 摘要:
            //     3 键。
            D3 = 51,
            //
            // 摘要:
            //     4 键。
            D4 = 52,
            //
            // 摘要:
            //     5 键。
            D5 = 53,
            //
            // 摘要:
            //     6 键。
            D6 = 54,
            //
            // 摘要:
            //     7 键。
            D7 = 55,
            //
            // 摘要:
            //     8 键。
            D8 = 56,
            //
            // 摘要:
            //     9 键。
            D9 = 57,
            //
            // 摘要:
            //     A 键。
            A = 65,
            //
            // 摘要:
            //     B 键。
            B = 66,
            //
            // 摘要:
            //     C 键。
            C = 67,
            //
            // 摘要:
            //     D 键。
            D = 68,
            //
            // 摘要:
            //     E 键。
            E = 69,
            //
            // 摘要:
            //     F 键。
            F = 70,
            //
            // 摘要:
            //     G 键。
            G = 71,
            //
            // 摘要:
            //     H 键。
            H = 72,
            //
            // 摘要:
            //     I 键。
            I = 73,
            //
            // 摘要:
            //     J 键。
            J = 74,
            //
            // 摘要:
            //     K 键。
            K = 75,
            //
            // 摘要:
            //     L 键。
            L = 76,
            //
            // 摘要:
            //     M 键。
            M = 77,
            //
            // 摘要:
            //     N 键。
            N = 78,
            //
            // 摘要:
            //     O 键。
            O = 79,
            //
            // 摘要:
            //     P 键。
            P = 80,
            //
            // 摘要:
            //     Q 键。
            Q = 81,
            //
            // 摘要:
            //     R 键。
            R = 82,
            //
            // 摘要:
            //     S 键。
            S = 83,
            //
            // 摘要:
            //     T 键。
            T = 84,
            //
            // 摘要:
            //     U 键。
            U = 85,
            //
            // 摘要:
            //     V 键。
            V = 86,
            //
            // 摘要:
            //     W 键。
            W = 87,
            //
            // 摘要:
            //     X 键。
            X = 88,
            //
            // 摘要:
            //     Y 键。
            Y = 89,
            //
            // 摘要:
            //     Z 键。
            Z = 90,
            //
            // 摘要:
            //     左 Windows 徽标键 (Microsoft Natural Keyboard)。
            LWin = 91,
            //
            // 摘要:
            //     右 Windows 徽标键 (Microsoft Natural Keyboard)。
            RWin = 92,
            //
            // 摘要:
            //     应用程序密钥 (Microsoft Natural Keyboard)。
            Apps = 93,
            //
            // 摘要:
            //     计算机休眠键。
            Sleep = 95,
            //
            // 摘要:
            //     数字键盘上的 0 键。
            NumPad0 = 96,
            //
            // 摘要:
            //     数字键盘上的 1 键。
            NumPad1 = 97,
            //
            // 摘要:
            //     数字键盘上的 2 键。
            NumPad2 = 98,
            //
            // 摘要:
            //     数字键盘上的 3 键。
            NumPad3 = 99,
            //
            // 摘要:
            //     数字键盘上的 4 键。
            NumPad4 = 100,
            //
            // 摘要:
            //     数字键盘上的 5 键。
            NumPad5 = 101,
            //
            // 摘要:
            //     数字键盘上的 6 键。
            NumPad6 = 102,
            //
            // 摘要:
            //     数字键盘上的 7 键。
            NumPad7 = 103,
            //
            // 摘要:
            //     数字键盘上的 8 键。
            NumPad8 = 104,
            //
            // 摘要:
            //     数字键盘上的 9 键。
            NumPad9 = 105,
            //
            // 摘要:
            //     乘号键。
            Multiply = 106,
            //
            // 摘要:
            //     加号键。
            Add = 107,
            //
            // 摘要:
            //     分隔符键。
            Separator = 108,
            //
            // 摘要:
            //     减号键。
            Subtract = 109,
            //
            // 摘要:
            //     句点键。
            Decimal = 110,
            //
            // 摘要:
            //     除号键。
            Divide = 111,
            //
            // 摘要:
            //     F1 键。
            F1 = 112,
            //
            // 摘要:
            //     F2 键。
            F2 = 113,
            //
            // 摘要:
            //     F3 键。
            F3 = 114,
            //
            // 摘要:
            //     F4 键。
            F4 = 115,
            //
            // 摘要:
            //     F5 键。
            F5 = 116,
            //
            // 摘要:
            //     F6 键。
            F6 = 117,
            //
            // 摘要:
            //     F7 键。
            F7 = 118,
            //
            // 摘要:
            //     F8 键。
            F8 = 119,
            //
            // 摘要:
            //     F9 键。
            F9 = 120,
            //
            // 摘要:
            //     F10 键。
            F10 = 121,
            //
            // 摘要:
            //     F11 键。
            F11 = 122,
            //
            // 摘要:
            //     F12 键。
            F12 = 123,
            //
            // 摘要:
            //     F13 键。
            F13 = 124,
            //
            // 摘要:
            //     F14 键。
            F14 = 125,
            //
            // 摘要:
            //     F15 键。
            F15 = 126,
            //
            // 摘要:
            //     F16 键。
            F16 = 127,
            //
            // 摘要:
            //     F17 键。
            F17 = 128,
            //
            // 摘要:
            //     F18 键。
            F18 = 129,
            //
            // 摘要:
            //     F19 键。
            F19 = 130,
            //
            // 摘要:
            //     F20 键。
            F20 = 131,
            //
            // 摘要:
            //     F21 键。
            F21 = 132,
            //
            // 摘要:
            //     F22 键。
            F22 = 133,
            //
            // 摘要:
            //     F23 键。
            F23 = 134,
            //
            // 摘要:
            //     F24 键。
            F24 = 135,
            //
            // 摘要:
            //     NUM LOCK 键。
            NumLock = 144,
            //
            // 摘要:
            //     SCROLL LOCK 键。
            Scroll = 145,
            //
            // 摘要:
            //     左的 SHIFT 键。
            LShiftKey = 160,
            //
            // 摘要:
            //     右 SHIFT 键。
            RShiftKey = 161,
            //
            // 摘要:
            //     左 CTRL 键。
            LControlKey = 162,
            //
            // 摘要:
            //     右 CTRL 键。
            RControlKey = 163,
            //
            // 摘要:
            //     左 ALT 键。
            LMenu = 164,
            //
            // 摘要:
            //     右 ALT 键。
            RMenu = 165,
            //
            // 摘要:
            //     浏览器后退键 (Windows 2000 或更高版本)。
            BrowserBack = 166,
            //
            // 摘要:
            //     浏览器前进键 (Windows 2000 或更高版本)。
            BrowserForward = 167,
            //
            // 摘要:
            //     浏览器刷新键 (Windows 2000 或更高版本)。
            BrowserRefresh = 168,
            //
            // 摘要:
            //     浏览器停止键 (Windows 2000 或更高版本)。
            BrowserStop = 169,
            //
            // 摘要:
            //     浏览器搜索键 (Windows 2000 或更高版本)。
            BrowserSearch = 170,
            //
            // 摘要:
            //     浏览器收藏键 (Windows 2000 或更高版本)。
            BrowserFavorites = 171,
            //
            // 摘要:
            //     浏览器主页键 (Windows 2000 或更高版本)。
            BrowserHome = 172,
            //
            // 摘要:
            //     卷静音键 (Windows 2000 或更高版本)。
            VolumeMute = 173,
            //
            // 摘要:
            //     音量降低键 (Windows 2000 或更高版本)。
            VolumeDown = 174,
            //
            // 摘要:
            //     音量增大键 (Windows 2000 或更高版本)。
            VolumeUp = 175,
            //
            // 摘要:
            //     媒体下一曲目键 (Windows 2000 或更高版本)。
            MediaNextTrack = 176,
            //
            // 摘要:
            //     媒体上一曲目键 (Windows 2000 或更高版本)。
            MediaPreviousTrack = 177,
            //
            // 摘要:
            //     媒体停止键 (Windows 2000 或更高版本)。
            MediaStop = 178,
            //
            // 摘要:
            //     在媒体播放暂停键 (Windows 2000 或更高版本)。
            MediaPlayPause = 179,
            //
            // 摘要:
            //     启动邮件键 (Windows 2000 或更高版本)。
            LaunchMail = 180,
            //
            // 摘要:
            //     选择媒体键 (Windows 2000 或更高版本) 中。
            SelectMedia = 181,
            //
            // 摘要:
            //     启动应用程序一个键 (Windows 2000 或更高版本)。
            LaunchApplication1 = 182,
            //
            // 摘要:
            //     启动应用程序两个键 (Windows 2000 或更高版本)。
            LaunchApplication2 = 183,
            //
            // 摘要:
            //     美国标准键盘 (Windows 2000 或更高版本) 上的 OEM 分号键。
            OemSemicolon = 186,
            //
            // 摘要:
            //     OEM 1 键。
            //Oem1 = 186,
            //
            // 摘要:
            //     OEM 加上任何国家/地区键盘 (Windows 2000 或更高版本) 上的密钥。
            Oemplus = 187,
            //
            // 摘要:
            //     任何国家/地区键盘 (Windows 2000 或更高版本) 上的 OEM 逗号键。
            Oemcomma = 188,
            //
            // 摘要:
            //     OEM 减号键 (Windows 2000 或更高版本) 任何国家/地区键盘上。
            OemMinus = 189,
            //
            // 摘要:
            //     任何国家/地区键盘 (Windows 2000 或更高版本) 上的 OEM 期间键。
            OemPeriod = 190,
            //
            // 摘要:
            //     美国标准键盘 (Windows 2000 或更高版本) 上的 OEM 问号键。
            OemQuestion = 191,
            //
            // 摘要:
            //     OEM 2 键。
            //Oem2 = 191,
            //
            // 摘要:
            //     美国标准键盘 (Windows 2000 或更高版本) 上的 OEM 颚化符键。
            Oemtilde = 192,
            //
            // 摘要:
            //     OEM 3 键。
            //Oem3 = 192,
            //
            // 摘要:
            //     美国标准键盘 (Windows 2000 或更高版本) 上的 OEM 左大括号键。
            OemOpenBrackets = 219,
            //
            // 摘要:
            //     OEM 4 键。
            //Oem4 = 219,
            //
            // 摘要:
            //     美国标准键盘 (Windows 2000 或更高版本) 上的 OEM 管道键。
            OemPipe = 220,
            //
            // 摘要:
            //     OEM 5 键。
            //Oem5 = 220,
            //
            // 摘要:
            //     美国标准键盘 (Windows 2000 或更高版本) 上的 OEM 右大括号键。
            OemCloseBrackets = 221,
            //
            // 摘要:
            //     OEM 6 键。
            //Oem6 = 221,
            //
            // 摘要:
            //     OEM 意见/双精度型引号密钥美国标准键盘 (Windows 2000 或更高版本) 上。
            OemQuotes = 222,
            //
            // 摘要:
            //     OEM 7 键。
            //Oem7 = 222,
            //
            // 摘要:
            //     OEM 8 键。
            Oem8 = 223,
            //
            // 摘要:
            //     OEM 尖括号或 RT 102 键键盘 (Windows 2000 或更高版本) 上的反斜杠键。
            OemBackslash = 226,
            //
            // 摘要:
            //     OEM 102 键。
            Oem102 = 226,
            //
            // 摘要:
            //     PROCESS 键键中。
            ProcessKey = 229,
            //
            // 摘要:
            //     用于传递 Unicode 字符，就像它们是击键一样。 Packet 键值是用于非键盘输入方法的 32 位虚拟密钥值的低位字。
            Packet = 231,
            //
            // 摘要:
            //     ATTN 键。
            Attn = 246,
            //
            // 摘要:
            //     CRSEL 键。
            Crsel = 247,
            //
            // 摘要:
            //     EXSEL 键。
            Exsel = 248,
            //
            // 摘要:
            //     ERASE EOF 键。
            EraseEof = 249,
            //
            // 摘要:
            //     播放键。
            Play = 250,
            //
            // 摘要:
            //     缩放键。
            Zoom = 251,
            //
            // 摘要:
            //     留待将来使用的常数。
            //NoName = 252,
            //
            // 摘要:
            //     PA1 键。
            Pa1 = 253,
            //
            // 摘要:
            //     CLEAR 键。
            OemClear = 254,
            //
            // 摘要:
            //     从一个密钥值中提取键代码的位屏蔽。
            //KeyCode = 65535,
            //
            // 摘要:
            //     SHIFT 修改键。
            //Shift = 65536,
            //
            // 摘要:
            //     CTRL 修改键。
            //Control = 131072,
            //
            // 摘要:
            //     ALT 修改键。
            //Alt = 262144
        }
    }
}
