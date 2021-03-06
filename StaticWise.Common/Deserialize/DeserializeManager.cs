﻿using StaticWise.Entities;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using StaticWise.Common.Files;
using System.IO;
using System.Globalization;
using System;

namespace StaticWise.Common.Deserialize
{
    public class DeserializeManager : IDeserializeManager
    {
        #region Constants

        private const string REGEX_FRONT_MATTER = @"^(\{[\s\S]*?\n\})(\s*\n)*";
        private const string FILE_DATE_FORMAT = "yyyyMMdd";

        #endregion

        #region Properties

        IFileManager _fileManager;

        #endregion

        #region Constructors

        public DeserializeManager(IFileManager fileManager)
        {
            _fileManager = fileManager;
        }

        #endregion

        #region Methods

        Config IDeserializeManager.DeserializeConfig(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return new Config();

            try
            {
                string json = _fileManager.GetTextFromFile(filePath);

                if (!string.IsNullOrEmpty(json))
                {
                    Config config = JsonConvert.DeserializeObject<Config>(json);
                    config.RootPath = Path.GetDirectoryName(filePath);
                    config.FilePath = filePath;

                    return config;
                }
                else
                    return new Config();
            }
            catch
            {
                return new Config();
            }
        }

        Post IDeserializeManager.DeserializePost(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return new Post();

            try
            {
                string content = _fileManager.GetTextFromFile(filePath);

                if (string.IsNullOrEmpty(content))
                    return new Post();

                Match match = Regex.Match(content,
                    REGEX_FRONT_MATTER,
                    RegexOptions.IgnoreCase);

                if (match.Success)
                {
                    Post post = JsonConvert.DeserializeObject<Post>(match.Value);
                    post.FileContent = content.Replace(match.Value, string.Empty);
                    post.FileDate = File.GetCreationTime(filePath);
                    post.FilePath = filePath;

                    // Get the filename
                    string fileName = Path.GetFileName(filePath);

                    if (fileName.Contains("-"))
                    {
                        // Get the published date as a string
                        string publishedDateRaw = fileName.Substring(0, fileName.IndexOf('-'));

                        if (DateTime.TryParseExact(publishedDateRaw, FILE_DATE_FORMAT,
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime publishedDate))
                            post.FileDate = publishedDate;
                    }

                    return post;
                }
                else
                    return new Post();
            }
            catch
            {
                return new Post();
            }
        }

        Page IDeserializeManager.DeserializePage(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return new Page();

            try
            {
                string content = _fileManager.GetTextFromFile(filePath);

                if (string.IsNullOrEmpty(content))
                    return new Page();

                Match match = Regex.Match(content,
                    REGEX_FRONT_MATTER, 
                    RegexOptions.IgnoreCase);

                if (match.Success)
                {
                    Page page = JsonConvert.DeserializeObject<Page>(match.Value);
                    page.FileContent = content.Replace(match.Value, string.Empty);
                    page.FilePath = filePath;

                    return page;
                }
                else
                    return new Page();
            }
            catch
            {
                return new Page();
            }
        }

        #endregion
    }
}