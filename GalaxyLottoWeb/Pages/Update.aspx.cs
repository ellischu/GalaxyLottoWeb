using GalaxyLotto.ClassLibrary;
using System;

namespace GalaxyLottoWeb.Pages
{
    public partial class Update : BasePage
    {
        private StuGLSearch _stuGLSearch = new StuGLSearch(TargetTable.Lotto539);
#pragma warning disable CA1707 // Identifiers should not contain underscores
        protected void Page_Load(object sender, EventArgs e)
#pragma warning restore CA1707 // Identifiers should not contain underscores
        {
            Form.Target = "_blank";
            //Action = Properties.Resources.SessionsUpdate;
            //RequestId = _stuGLSearch.LottoType.ToString();
            if (Session["action"] == null)
            {
                Session["action"] = Properties.Resources.SessionsUpdate;                
            }
            Page.Form.Attributes.Add("enctype", "multipart/form-data");
        }

        protected void BtnBigClick(object sender, EventArgs e)
        {
            _stuGLSearch.LottoType = TargetTable.LottoBig;
            Session["action"] = Properties.Resources.SessionsUpdate;
            Session["id"] = _stuGLSearch.LottoType.ToString();
            Session[Properties.Resources.SessionsUpdate + _stuGLSearch.LottoType.ToString()] = _stuGLSearch;
        }

        protected void Btn539Click(object sender, EventArgs e)
        {
            _stuGLSearch.LottoType = TargetTable.Lotto539;
            Session["action"] = Properties.Resources.SessionsUpdate;
            Session["id"] = _stuGLSearch.LottoType.ToString();
            Session[Properties.Resources.SessionsUpdate + _stuGLSearch.LottoType.ToString()] = _stuGLSearch;
        }

        protected void BtnDafuClick(object sender, EventArgs e)
        {
            _stuGLSearch.LottoType = TargetTable.LottoDafu;
            Session["action"] = Properties.Resources.SessionsUpdate;
            Session["id"] = _stuGLSearch.LottoType.ToString();
            Session[Properties.Resources.SessionsUpdate + _stuGLSearch.LottoType.ToString()] = _stuGLSearch;
        }

        protected void BtnWeliClick(object sender, EventArgs e)
        {
            _stuGLSearch.LottoType = TargetTable.LottoWeli;
            Session["action"] = Properties.Resources.SessionsUpdate;
            Session["id"] = _stuGLSearch.LottoType.ToString();
            Session[Properties.Resources.SessionsUpdate + _stuGLSearch.LottoType.ToString()] = _stuGLSearch;
        }

        protected void BtnSixClick(object sender, EventArgs e)
        {
            _stuGLSearch.LottoType = TargetTable.LottoSix;
            Session["action"] = Properties.Resources.SessionsUpdate;
            Session["id"] = _stuGLSearch.LottoType.ToString();
            Session[Properties.Resources.SessionsUpdate + _stuGLSearch.LottoType.ToString()] = _stuGLSearch;
        }
        protected void BtnTwinWinClick(object sender, EventArgs e)
        {
            _stuGLSearch.LottoType = TargetTable.LottoTwinWin;
            Session["action"] = Properties.Resources.SessionsUpdate;
            Session["id"] = _stuGLSearch.LottoType.ToString();
            Session[Properties.Resources.SessionsUpdate + _stuGLSearch.LottoType.ToString()] = _stuGLSearch;
        }

        protected void BtnWClick(object sender, EventArgs e)
        {
            _stuGLSearch.LottoType = TargetTable.DateWC;
            Session["action"] = Properties.Resources.SessionsUpdate;
            Session["id"] = _stuGLSearch.LottoType.ToString();
            Session[Properties.Resources.SessionsUpdate + _stuGLSearch.LottoType.ToString()] = _stuGLSearch;
        }

        protected void BtnPurpleClick(object sender, EventArgs e)
        {
            _stuGLSearch.LottoType = TargetTable.DataPurple;
            Session["action"] = Properties.Resources.SessionsUpdate;
            Session["id"] = _stuGLSearch.LottoType.ToString();
            Session[Properties.Resources.SessionsUpdate + _stuGLSearch.LottoType.ToString()] = _stuGLSearch;
        }

        protected void BtnOptionsClick(object sender, EventArgs e)
        {
            _stuGLSearch.LottoType = TargetTable.DataOption;
            Session["action"] = Properties.Resources.SessionsUpdate;
            Session["id"] = _stuGLSearch.LottoType.ToString();
            Session[Properties.Resources.SessionsUpdate + _stuGLSearch.LottoType.ToString()] = _stuGLSearch;
        }


        protected void Btn539OnLineClick(object sender, EventArgs e)
        {
            new CglFunc().TWLotteryUpadteOnLine(TargetTable.Lotto539);
        }

        protected void BtnBigOnLineClick(object sender, EventArgs e)
        {
            new CglFunc().TWLotteryUpadteOnLine(TargetTable.LottoBig);
        }

        protected void BtnDafuOnLineClick(object sender, EventArgs e)
        {
            new CglFunc().TWLotteryUpadteOnLine(TargetTable.LottoDafu);
        }

        protected void BtnWeliOnLineClick(object sender, EventArgs e)
        {
            new CglFunc().TWLotteryUpadteOnLine(TargetTable.LottoWeli);
        }

        protected void BtnTwinWinOnLineClick(object sender, EventArgs e)
        {
            new CglFunc().TWLotteryUpadteOnLine(TargetTable.LottoTwinWin);
        }

        protected void BtnUpdateClick(object sender, EventArgs e)
        {
            // Specify the path on the server to
            // save the uploaded file to.
            string savePath = @"d:\temp\uploads\";

            // Before attempting to perform operations
            // on the file, verify that the FileUpload 
            // control contains a file.
            if (FileInput.HasFile)
            {
                // Get the name of the file to upload.
                string fileName = FileInput.FileName;

                // Append the name of the file to upload to the path.
                savePath += fileName;


                // Call the SaveAs method to save the 
                // uploaded file to the specified path.
                // This example does not perform all
                // the necessary error checking.               
                // If a file with the same name
                // already exists in the specified path,  
                // the uploaded file overwrites it.
                FileInput.SaveAs(savePath);

                // Notify the user of the name of the file
                // was saved under.
                UploadStatusLabel.Text = string.Format(InvariantCulture, "Your file was saved as {0}", fileName);
            }
            else
            {
                // Notify the user that a file was not uploaded.
                UploadStatusLabel.Text = string.Format(InvariantCulture, "You did not specify a file to upload.");
            }

            var lottos = ddlLottoType.SelectedValue switch
            {
                "539" => TargetTable.Lotto539,
                "Big" => TargetTable.LottoBig,
                "Weli" => TargetTable.LottoWeli,
                _ => TargetTable.LottoTwinWin,
            };
            string strResult = System.IO.File.ReadAllText(savePath);
            new CglFunc().UpdateDataSilent(lottos, new CglFunc().GetTaiwanLottoUpdateTableOL(lottos, strResult));
        }
    }
}