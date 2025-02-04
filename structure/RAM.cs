﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Buffers.Binary.BinaryPrimitives;

namespace _3DS_link_trade_bot
{
    public class RAM
    {
        //gen7
        public static uint Friendslistoffset = 0x30011134;
        public static int FriendListSize = 0x8FC0;
        public static uint friendsize = 0x2E0;
        public static uint namestart = 0x18;
        public static uint isconnectedoff = 0x318C5A12;
        public static uint L_inlocalwireless = 0x6c;
        public static bool isconnected => Form1.ntr.ReadBytes(isconnectedoff, 1)[0] != L_inlocalwireless;

        public static uint FailedTradeoff = 0x3023E34C;
        public static bool failedtrade => Form1.ntr.ReadBytes(FailedTradeoff, 1)[0] == 0x64;
        public static uint OfferedPokemonoff = 0x006A6DD4;
        public static uint finalofferscreenoff = 0x30192EEA;
        public static uint box1slot1 = 0x33015AB0;
        public static uint tradevolutionscreenoff = 0x3002040C;
        public static bool tradeevolution => Form1.ntr.ReadBytes(tradevolutionscreenoff, 1)[0] == 0x57;
        public static uint screenoff = 0x006A610A;
        public static uint boxscreen = 0x4120;
        public static uint start_seekscreen = 0x3F2B;
        public static bool onboxscreen => BitConverter.ToUInt16(Form1.ntr.ReadBytes(screenoff, 2)) == boxscreen;
        public static uint GTSpagesizeoff = 0x329921A4;
        public static uint GTSblockoff = 0x329927C4;
        public static uint GTScurrentview = 0x305CD9F4;
        public static uint GTSDeposit = 0x32992180;
        public static uint festscreenoff = 0x318CBFEC;
        public static int festscreendisplayed = 0x38;
        public static uint Userinvitedbotscreenoff = 0x31928D74;
        public static int userinvitedbotscreenval = 0x21;
        public static bool userinvitedbot => Form1.ntr.ReadBytes(Userinvitedbotscreenoff, 1)[0] == userinvitedbotscreenval;

        public static bool infestivalplaza => Form1.ntr.ReadBytes(festscreenoff, 1)[0] == festscreendisplayed;

        public static uint SoftBanOff = 0x32994352;
        public static int softbanscreen = 0x50;
        public static bool IsSoftBanned => Form1.ntr.ReadBytes(SoftBanOff, 1)[0] == softbanscreen;

        public static uint WTReceivingPokemon = 0x314FF4D1;
        public static uint WTTrainerMatch = 0x303987B4;

        //gen6
        public static uint PSSFriendoff = 0x08C6FFDC;
        public static uint PSSBlockSize = 0x4E30;
        public static uint PSSDataSize = 0x4E20;
    }
    public readonly ref struct FriendList
    {
        public const int friendlistsize = 0x8FC0;
        public const int numofguests = 50;
        private readonly Span<byte> Data;
        public FriendList(Span<byte> data) => Data = data;
        public friend this[int index]=>new(Data.Slice(friend.friendsize*index,friend.friendsize));

    }
    public readonly ref struct friend
    {
        public const int friendsize = 0x2E0;
        private readonly Span<byte> Data;
        public friend(Span<byte> data) => Data = data;
      
        public string friendname => Encoding.Unicode.GetString(Data.Slice(24,24)).Trim('\0');
    }
    public readonly ref struct GTSPage
    {
        public const int GTSBlocksize = 0x6400;
        private readonly Span<byte> Data;
        public GTSPage(Span<byte> data) => Data = data;
        public GTSEntry this[int index] => new(Data.Slice(GTSEntry.GTSEntrySize * index, GTSEntry.GTSEntrySize));
    }
    public readonly ref struct GTSEntry
    {
        public const int GTSEntrySize = 0x100;
        private readonly Span<byte> Data;
        public GTSEntry(Span<byte> data) => Data = data;
        public int RequestedPoke => BitConverter.ToInt16(Data[0xC..]);
        public string trainername => Encoding.Unicode.GetString(Data.Slice(0x4c,24)).Trim('\0');
        public int genderindex => Data[0xE];
        public int levelindex => Data[0xF];


    }
    public readonly ref struct PSSfriendlist
    {

        private readonly Span<byte> Data;
        public PSSfriendlist(Span<byte> data) => Data = data;
        public PSSFriend this[int index] => new(Data.Slice(PSSFriend.friendsize*index,PSSFriend.friendsize));
        

    }
    public readonly ref struct PSSFriend
    {
        private readonly Span<byte> Data;
        public PSSFriend(Span<byte> data) => Data = data;
        public const int friendsize = 0xc8;
         public ulong pssID => ReadUInt64LittleEndian(Data);
        
        public string otname => Encoding.Unicode.GetString(Data.Slice(8, 24)).Trim('\0');
    }
}
