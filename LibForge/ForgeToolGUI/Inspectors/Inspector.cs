﻿using LibForge.Lipsync;
using LibForge.Mesh;
using LibForge.Midi;
using LibForge.RBSong;
using LibForge.SongData;
using LibForge.Texture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ForgeToolGUI
{
  static class InspectorFactory
  {
    /// <summary>
    /// Returns an inspector for the object or null if there isn't one.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static Inspector GetInspector(object obj)
    {
      switch (obj)
      {
        case System.Drawing.Image i:
          return new ImageInspector(i);
        case string s:
          return new StringInspector(s);
        case SongData d:
          return new SongDataInspector(d);
        case RBMid.GEMTRACK g:
          return new GemTrackInspector(g);
        case object o:
          return new ObjectInspector(obj);
      }
      return null;
    }


    public static object LoadObject(GameArchives.IFile i)
    {
      if (i.Name.Contains(".bmp_") || i.Name.Contains(".png_"))
      {
        using (var s = i.GetStream())
        {
          try
          {
            var tex = TextureReader.ReadStream(s);
            return TextureConverter.ToBitmap(tex, 0);
          }
          catch (Exception ex)
          {
            System.Windows.Forms.MessageBox.Show("Couldn't load texture: " + ex.Message);
            return null;
          }
        }
      }
      else if (i.Name.Contains("_dta_") || i.Name.EndsWith(".dtb"))
      {
        using (var s = i.GetStream())
        {
          var data = DtxCS.DTX.FromDtb(s);
          var sb = new StringBuilder();
          foreach (var x in data.Children)
          {
            sb.AppendLine(x.ToString(0));
          }
          return sb.ToString();
        }
      }
      else if (i.Name.EndsWith(".dta") || i.Name.EndsWith(".moggsong"))
      {
        using (var s = i.GetStream())
        using (var r = new System.IO.StreamReader(s))
        {
          return r.ReadToEnd();
        }
      }
      else if (i.Name.Contains(".songdta"))
      {
        using (var s = i.GetStream())
        {
          var songData = SongDataReader.ReadStream(s);
          return songData;
        }
      }
      else if (i.Name.Contains(".fbx"))
      {
        using (var s = i.GetStream())
        {
          var mesh = HxMeshReader.ReadStream(s);
          return HxMeshConverter.ToObj(mesh);
        }
      }
      else if (i.Name.Contains(".rbmid_"))
      {
        using (var s = i.GetStream())
        {
          return RBMidReader.ReadStream(s);
        }
      }
      else if (i.Name.Contains(".lipsync"))
      {
        using (var s = i.GetStream())
        {
          return new LipsyncReader(s).Read();
        }
      }
      else if (i.Name.Contains(".rbsong"))
      {
        using (var s = i.GetStream())
        {
          return new RBSongReader(s).Read();
        }
      }
      else
      {
        return null;
      }
    }
  }

  public class Inspector : System.Windows.Forms.UserControl
  {
    protected ForgeBrowser fb;
    public void SetBrowser(ForgeBrowser f)
    {
      fb = f;
    }
  }
}