﻿using System;

namespace SabreTools.Library.DatFiles
{
    /// <summary>
    /// DAT output formats
    /// </summary>
    [Flags]
    public enum DatFormat
    {
        #region XML Formats

        /// <summary>
        /// Logiqx XML (using machine)
        /// </summary>
        Logiqx = 1 << 0,

        /// <summary>
        /// Logiqx XML (using game)
        /// </summary>
        LogiqxDeprecated = 1 << 1,

        /// <summary>
        /// MAME Softare List XML
        /// </summary>
        SoftwareList = 1 << 2,

        /// <summary>
        /// MAME Listxml output
        /// </summary>
        Listxml = 1 << 3,

        /// <summary>
        /// OfflineList XML
        /// </summary>
        OfflineList = 1 << 4,

        /// <summary>
        /// SabreDAT XML
        /// </summary>
        SabreXML = 1 << 5,

        /// <summary>
        /// openMSX Software List XML
        /// </summary>
        OpenMSX = 1 << 6,

        #endregion

        #region Propietary Formats

        /// <summary>
        /// ClrMamePro custom
        /// </summary>
        ClrMamePro = 1 << 7,

        /// <summary>
        /// RomCenter INI-based
        /// </summary>
        RomCenter = 1 << 8,

        /// <summary>
        /// DOSCenter custom
        /// </summary>
        DOSCenter = 1 << 9,

        /// <summary>
        /// AttractMode custom
        /// </summary>
        AttractMode = 1 << 10,

        #endregion

        #region Standardized Text Formats

        /// <summary>
        /// ClrMamePro missfile
        /// </summary>
        MissFile = 1 << 11,

        /// <summary>
        /// Comma-Separated Values (standardized)
        /// </summary>
        CSV = 1 << 12,

        /// <summary>
        /// Semicolon-Separated Values (standardized)
        /// </summary>
        SSV = 1 << 13,

        /// <summary>
        /// Tab-Separated Values (standardized)
        /// </summary>
        TSV = 1 << 14,

        /// <summary>
        /// MAME Listrom output
        /// </summary>
        Listrom = 1 << 15,

        /// <summary>
        /// Everdrive Packs SMDB
        /// </summary>
        EverdriveSMDB = 1 << 16,

        /// <summary>
        /// SabreJSON
        /// </summary>
        SabreJSON = 1 << 17,

        #endregion

        #region SFV-similar Formats

        /// <summary>
        /// CRC32 hash list
        /// </summary>
        RedumpSFV = 1 << 18,

        /// <summary>
        /// MD5 hash list
        /// </summary>
        RedumpMD5 = 1 << 19,

#if NET_FRAMEWORK
        /// <summary>
        /// RIPEMD160 hash list
        /// </summary>
        RedumpRIPEMD160 = 1 << 20,
#endif

        /// <summary>
        /// SHA-1 hash list
        /// </summary>
        RedumpSHA1 = 1 << 21,

        /// <summary>
        /// SHA-256 hash list
        /// </summary>
        RedumpSHA256 = 1 << 22,

        /// <summary>
        /// SHA-384 hash list
        /// </summary>
        RedumpSHA384 = 1 << 23,

        /// <summary>
        /// SHA-512 hash list
        /// </summary>
        RedumpSHA512 = 1 << 24,

        /// <summary>
        /// SpamSum hash list
        /// </summary>
        RedumpSpamSum = 1 << 25,

        #endregion

        // Specialty combinations
        ALL = Int32.MaxValue,
    }

    /// <summary>
    /// Determines the DAT deduplication type
    /// </summary>
    public enum DedupeType
    {
        None = 0,
        Full,

        // Force only deduping with certain types
        Game,
        CRC,
        MD5,
#if NET_FRAMEWORK
        RIPEMD160,
#endif
        SHA1,
        SHA256,
        SHA384,
        SHA512,
    }

    /// <summary>
    /// Available hashing types
    /// </summary>
    [Flags]
    public enum Hash
    {
        CRC = 1 << 0,
        MD5 = 1 << 1,
#if NET_FRAMEWORK
        RIPEMD160 = 1 << 2,
#endif
        SHA1 = 1 << 3,
        SHA256 = 1 << 4,
        SHA384 = 1 << 5,
        SHA512 = 1 << 6,
        SpamSum = 1 << 7,

        // Special combinations
        Standard = CRC | MD5 | SHA1,
#if NET_FRAMEWORK
        DeepHashes = RIPEMD160 | SHA256 | SHA384 | SHA512 | SpamSum,
        SecureHashes = MD5 | RIPEMD160 | SHA1 | SHA256 | SHA384 | SHA512 | SpamSum,
#else
        DeepHashes = SHA256 | SHA384 | SHA512 | SpamSum,
        SecureHashes = MD5 | SHA1 | SHA256 | SHA384 | SHA512 | SpamSum,
#endif
    }

    /// <summary>
    /// Determines merging tag handling for DAT output
    /// </summary>
    public enum MergingFlag
    {
        None = 0,
        Split,
        Merged,
        NonMerged,
        Full,

        /// <remarks>This is not usually defined for Merging flags</remarks>
        Device,
    }

    /// <summary>
    /// Determines nodump tag handling for DAT output
    /// </summary>
    public enum NodumpFlag
    {
        None = 0,
        Obsolete,
        Required,
        Ignore,
    }

    /// <summary>
    /// Determines packing tag handling for DAT output
    /// </summary>
    public enum PackingFlag
    {
        None = 0,

        /// <summary>
        /// Force all sets to be in archives, except disk and media
        /// </summary>
        Zip,

        /// <summary>
        /// Force all sets to be extracted into subfolders
        /// </summary>
        Unzip,

        /// <summary>
        /// Force sets with single items to be extracted to the parent folder
        /// </summary>
        Partial,

        /// <summary>
        /// Force all sets to be extracted to the parent folder
        /// </summary>
        Flat,
    }

    /// <summary>
    /// Determines which files should be skipped in DFD
    /// </summary>
    public enum SkipFileType
    {
        None = 0,
        Archive,
        File,
    }

    /// <summary>
    /// Determines what sort of files get externally hashed
    /// </summary>
    /// TODO: Can FileType be used instead?
    [Flags]
    public enum TreatAsFile
    {
        CHD = 1 << 0,
        Archive = 1 << 1,
        AaruFormat = 1 << 2,

        NonArchive = CHD | AaruFormat,
        All = CHD | Archive | AaruFormat,
    }
}
