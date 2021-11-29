using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DotVVM.Framework.ViewModel;
using DotVVM.Framework.Hosting;
using DotVVM.Core.Storage;
using DotVVM.Framework.Controls;
using System.IO;

namespace ImageClassification.ViewModels
{
    public class DefaultViewModel : MasterPageViewModel
    {
		private IUploadedFileStorage storage;
		public string Result { get; set; } = null;
		public decimal? Score { get; set; } = null;

		public UploadedFilesCollection Files { get; set; }


		public DefaultViewModel(IUploadedFileStorage storage)
		{
			// use dependency injection to request IUploadedFileStorage
			this.storage = storage;

			Files = new UploadedFilesCollection();
		}

		public void Predict()
		{

			var uploadPath = GetUploadPath();
			var targetPath = Path.Combine(uploadPath, Files.Files[0].FileId + ".bin");
			storage.SaveAsAsync(Files.Files[0].FileId, targetPath);

			var sampleData = new MLModel.ModelInput()
			{
				ImageSource = targetPath,
			};

			//Load model and predict output
			var output = MLModel.Predict(sampleData);
			Result = output.Prediction;

			if (Result.Equals("With mask"))
			{
				Score = decimal.Round((decimal)(output.Score[0]) * 100, 2);
			}
			else
			{
				Score = decimal.Round((decimal)(output.Score[1]) * 100, 2);
			}

			//storage.DeleteFileAsync(Files.Files[0].FileId);
			Files.Clear();
		}

		private string GetUploadPath()
		{
			var uploadPath = Path.Combine(Context.Configuration.ApplicationPhysicalPath, "MyFiles");
			if (!Directory.Exists(uploadPath))
			{
				Directory.CreateDirectory(uploadPath);
			}
			return uploadPath;
		}
	}
}
