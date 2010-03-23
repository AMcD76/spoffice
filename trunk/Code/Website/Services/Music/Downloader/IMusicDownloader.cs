﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spoffice.Website.Models;

namespace Spoffice.Website.Services.Music.Downloader
{
    public interface IMusicDownloader
    {
        void DownloadTrack(Track track);
    }
}
