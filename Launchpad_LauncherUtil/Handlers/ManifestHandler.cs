using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Launchpad.LauncherUtil
{
	public sealed class ManifestHandler
	{
		public delegate void ManifestProgressChangedEventHandler(object sender, ProgressEventArgs e);
		/// <summary>
		/// Occurs when file progress changed.
		/// </summary>
		public event ManifestProgressChangedEventHandler ManifestGenerationProgressChanged;

		public delegate void ManifestGenerationFinishedEventHandler (object sender, GenerationFinishedEventArgs e);
		/// <summary>
		/// Occurs when file download finished.
		/// </summary>
		public event ManifestGenerationFinishedEventHandler ManifestGenerationFinished;

		/// <summary>
		/// The progress arguments object. Is updated during file download operations.
		/// </summary>
		private ProgressEventArgs ProgressArgs;

		/// <summary>
		/// The download finished arguments object. Is updated once a file download finishes.
		/// </summary>
		private GenerationFinishedEventArgs GenerationFinishedArgs;

		public ManifestHandler ()
		{
			ProgressArgs = new ProgressEventArgs ();
			GenerationFinishedArgs = new GenerationFinishedEventArgs ();
		}

		public void GenerateManifest(string folderPath)
		{
			Thread t = new Thread (() => GenerateManifestAsync (folderPath));
			t.Start ();
		}
		private void GenerateManifestAsync(string folderPath)
		{
			try
			{
				//generate the manifest
				string manifestPath = String.Format(@"{0}{1}Workspace{1}LauncherManifest.txt", 
				                                    Directory.GetCurrentDirectory(), 
				                                    Path.DirectorySeparatorChar);

				if (!Directory.Exists(Path.GetDirectoryName(manifestPath)))
				{
					Directory.CreateDirectory(Path.GetDirectoryName(manifestPath));
				}

				//create a new empty file and close it (effectively deleting the old manifest)
				File.Create(manifestPath).Close();

				string[] files = Directory.GetFiles(folderPath, 
				                                    "*", 
				                                    SearchOption.AllDirectories); 

				IEnumerable<string> filePaths = Directory.EnumerateFiles(folderPath, 
				                                                         "*", 
				                                                         SearchOption.AllDirectories);

				int fileCount = files.Length;
				int completedFiles = 0;

				ProgressArgs.TotalFiles = fileCount;


				TextWriter tw = new StreamWriter(manifestPath);
				foreach (string filePath in filePaths)
				{
					if (filePath != null)
					{
						FileStream fileStream = File.OpenRead(filePath);


						string fileName = filePath.Substring(folderPath.Length);
						string fileSize = fileStream.Length.ToString();

						//GetFileHash is Stream Safe and closes the file on its own
						string manifestLine = String.Format(@"{0}:{1}:{2}", 
						                                    fileName,
						                                    GetFileHash(fileStream), 
						                                    fileSize);

						completedFiles++;

						ProgressArgs.Filename = fileName;
						ProgressArgs.IndexedFiles = completedFiles;

						tw.WriteLine(manifestLine);

						//raise the progress changed event
						OnGenerationProgressChanged ();
					}
				}
				tw.Close();


				//generate the manifest checksum
				Stream manifestStream = File.OpenRead (manifestPath);

				string manifestChecksumPath = String.Format ("{0}{1}Workspace{1}LauncherManifest.checksum", 
				                                             Directory.GetCurrentDirectory (),
				                                             Path.DirectorySeparatorChar);
				if (File.Exists(manifestChecksumPath))
				{
					//create a new empty file and close it (effectively deleting the old checksum)
					File.Create(manifestChecksumPath).Close();
				}

				TextWriter tw2 = new StreamWriter (manifestChecksumPath);
				tw2.WriteLine (GetFileHash (manifestStream));

				GenerationFinishedArgs.ManifestPath = manifestPath;
				GenerationFinishedArgs.ChecksumPath = manifestChecksumPath;
				GenerationFinishedArgs.ManifestSize = manifestStream.Length; 

				tw2.Close ();
				manifestStream.Close ();

				//raise the generation finished event
				OnManifestGenerationFinished ();
			}
			catch (Exception ex)
			{
				Console.WriteLine ("GenerateManifest(): " + ex.Message);
				Console.WriteLine (ex.StackTrace);
			}
		}

		
		//function that takes a file stream, then computes the MD5 hash for the stream
		public string GetFileHash(Stream fileStream)
		{
			try
			{
				using (var md5 = MD5.Create())
				{
					//we got a valid file, calculate the MD5 and return it
					var resultString = BitConverter.ToString(md5.ComputeHash(fileStream)).Replace("-", "");
					OnManifestGenerationFinished();

					return resultString;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine ("GetFileHash(): " + ex.Message);
				return "";
			}
		}

		/// <summary>
		/// Raises the generation progress changed event.
		/// </summary>
		private void OnGenerationProgressChanged()
		{
			if (ManifestGenerationProgressChanged != null)
			{
				//raise the event
				ManifestGenerationProgressChanged (this, ProgressArgs);
			}
		}

		/// <summary>
		/// Raises the generation finished event.
		/// </summary>
		private void OnManifestGenerationFinished()
		{
			if (ManifestGenerationFinished != null)
			{
				//raise the event
				Console.WriteLine ("Attempting to raise...");
				ManifestGenerationFinished (this, GenerationFinishedArgs);
			}
		}
	}

	/// <summary>
	/// Progress event arguments.
	/// </summary>
	public class ProgressEventArgs : EventArgs
	{
		public int IndexedFiles {
			get;
			set;
		}

		public int TotalFiles {
			get;
			set;
		}

		public int TotalBytes {
			get;
			set;
		}

		public string Filename {
			get;
			set;
		}

		new public void Empty()
		{
			IndexedFiles = 0;
			TotalBytes = 0;
			TotalFiles = 0;
			Filename = "";
		}
	}

	/// <summary>
	/// Download finished event arguments.
	/// </summary>
	public class GenerationFinishedEventArgs : EventArgs
	{
		public long ManifestSize 
		{
			get;
			set;
		}

		public string ManifestPath 
		{
			get;
			set;
		}

		public string ChecksumPath
		{
			get;
			set;
		}

		new public void Empty()
		{
			ManifestSize = 0;
			ManifestPath = "";
			ChecksumPath = "";
		}
	}
}

