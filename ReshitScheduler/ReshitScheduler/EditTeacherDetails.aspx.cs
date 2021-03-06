﻿
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ReshitScheduler
{
    public partial class EditTeacherDetails : BasePage
    {
        private DataTable dtTeachers;
        private DataTable dtClasses;
        private DataTable dtTeachersAccesses;
        private int nTeacherID;
        protected void Page_Load(object sender, EventArgs e)
        {
            dtTeachers = DBConnection.Instance.GetAllThisYearTeachers();
            dtClasses = DBConnection.Instance.GetThisYearClasses();
            dtTeachersAccesses = DBConnection.Instance.GetThisYearTeachersAccesses();
            if (!IsPostBack)
            {

                ddlTeachers.DataSource = dtTeachers;
                ddlTeachers.DataValueField = "id";
                ddlTeachers.DataTextField = "name";
                ddlTeachers.DataBind();

                DataTable dtTeacherTypes = DBConnection.Instance.GetDataTableForDisplay("teacher_types");

                ddlTeacherTypes.DataSource = dtTeacherTypes;
                ddlTeacherTypes.DataValueField = "id";
                ddlTeacherTypes.DataTextField = "name";
                ddlTeacherTypes.DataBind();

            }
            nTeacherID = Convert.ToInt32(ddlTeachers.SelectedValue);
            if (!IsPostBack)
            {
                FillTeacherDetails();

            }
            if (!IsPostBack || GetControlThatCausedPostBack(this) == BtnUpdateTeacher)
            {
                FillClasses();
            }

        }
        /// <summary>
        /// Retrieves the control that caused the postback.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        private Control GetControlThatCausedPostBack(Page page)
        {
            //initialize a control and set it to null
            Control ctrl = null;

            //get the event target name and find the control
            string ctrlName = page.Request.Params.Get("__EVENTTARGET");
            if (!String.IsNullOrEmpty(ctrlName))
                ctrl = page.FindControl(ctrlName);

            //return the control to the calling method
            return ctrl;
        }

        private void FillTeacherDetails()
        {
            DataRow drTeacherDetails = dtTeachers.Select("id = " + nTeacherID)[0];
            txtTeacherFirstName.Text = drTeacherDetails["first_name"].ToString();
            txtTeacherLastName.Text = drTeacherDetails["last_name"].ToString();
            txtUserName.Text = drTeacherDetails["user_name"].ToString();
            txtPassword.Text = drTeacherDetails["password"].ToString();
            ddlTeacherTypes.SelectedValue = drTeacherDetails["teacher_type_id"].ToString();
        }

        private void FillClasses()
        {
            CheckBox chkClass = null;
            DataRow[] drClassesAccesses = dtTeachersAccesses.Select("teacher_id = " + nTeacherID);
            foreach (DataRow drCurrentClass in dtClasses.Rows)
            {
                chkClass = new CheckBox()
                {
                    Text = drCurrentClass["name"].ToString(),
                    ID = drCurrentClass["class_id"].ToString(),
                    CssClass = "form-control-lg form-check form-check-inline col-6 col-sm-4 col-md-3 col-lg-2",
                    Checked = drClassesAccesses.Select(access => access["class_id"]).Contains(drCurrentClass["class_id"])

                };
                pnlClasses.Controls.Add(chkClass);
            }
        }


        protected void ddlTeachers_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillTeacherDetails();
            FillClasses();
        }
        protected void BtnBack_Click(object sender, EventArgs e)
        {
            GoBack();
        }
        private void GoBack()
        {
            Response.Redirect("MainForm.aspx");
        }

        protected void BtnUpdateTeacher_Click(object sender, EventArgs e)
        {
            string[] strFields = { "first_name","last_name","teacher_type_id","user_name","password"};
            string[] strValues = { txtTeacherFirstName.Text, txtTeacherLastName.Text, ddlTeacherTypes.SelectedValue, 
                               txtUserName.Text, txtPassword.Text};

            bool bUpdateSucceeded = DBConnection.Instance.UpdateTableRow("teachers", nTeacherID, strFields, strValues);
            if (!bUpdateSucceeded)
            {
                Helper.ShowMessage(ClientScript, "error saving");
            }
            else
            {
                string strDeleteQuery = "delete from teacher_class_access where teacher_id = " + nTeacherID;
                DBConnection.Instance.ExecuteNonQuery(strDeleteQuery);
                string strInsertCommand = "insert into teacher_class_access(teacher_id,class_id) values ";
                bool bIsGroupEmpty = true;

                foreach (Control ctrlCurrentControl in pnlClasses.Controls)
                {
                    if (ctrlCurrentControl is CheckBox && (ctrlCurrentControl as CheckBox).Checked)
                    {
                        bIsGroupEmpty = false;
                        strInsertCommand += "(" + nTeacherID + "," + ctrlCurrentControl.ID + "),";
                    }
                }
                if (!bIsGroupEmpty)
                {
                    strInsertCommand = strInsertCommand.Remove(strInsertCommand.Length - 1);
                    DBConnection.Instance.ExecuteNonQuery(strInsertCommand);
                }
            }
            Helper.ShowMessage(ClientScript, "שינויים נשמרו");
            ResetTeacherDetails();

        }
        

        protected void BtnDeleteTeacher(object sender, EventArgs e)
        {
            string confirmValue = Request.Form["confirm_value"];
            if (confirmValue == "Yes")
            {
                DeleteTeacher();
            }
        }
        private void DeleteTeacher()
        {
            int nCurrentYearID = DBConnection.Instance.GetCurrentYearID();
            bool bCheckTables = CheckTables("teacher_class_access", "לא ניתן למחוק מורה בעל גישה לכיתות");
            if(!bCheckTables)
                return;
            bCheckTables = CheckTables("groups", "לא ניתן למחוק מורה שמלמד קבוצות");
            if (!bCheckTables)
                return;
            bCheckTables = CheckTables("courses", "לא ניתן למחוק מורה שמלמד שיעורים");
            if (!bCheckTables)
                return;
            bCheckTables = CheckTables("classes", "לא ניתן למחוק מורה שמחנך כיתה");
            if (!bCheckTables)
                return;
            string strDeleteQuery = " delete from teachers where id = " + nTeacherID + " and year_id = " + nCurrentYearID + ";";
            DBConnection.Instance.ExecuteNonQuery(strDeleteQuery);
            Helper.ShowMessage(ClientScript, "מורה נמחק");
            ResetTeacherDetails();
        }

        private bool CheckTables(string strTableName, string message)
        {
            DataTable dtCheckTables;
            int nCurrentYearID = DBConnection.Instance.GetCurrentYearID();
            dtCheckTables = DBConnection.Instance.GetDataTableByQuery("select " + strTableName + ".teacher_id from " + strTableName +
                                                                    " inner join teachers on " + strTableName + ".teacher_id = teachers.id" +
                                                                    " where " + strTableName + ".teacher_id = " + nTeacherID +
                                                                    " and teachers.year_id = " + nCurrentYearID);
            if (dtCheckTables.Rows.Count != 0)
            {
                Helper.ShowMessage(ClientScript, message);
                ResetTeacherDetails();
                return false;
            }
            return true;
        }

        private void ResetTeacherDetails()
        {
            dtTeachers = DBConnection.Instance.GetAllThisYearTeachers();
            ddlTeachers.DataSource = dtTeachers;
            ddlTeachers.DataValueField = "id";
            ddlTeachers.DataTextField = "name";
            ddlTeachers.DataBind();
            ddlTeachers.SelectedIndex = 0;
            nTeacherID = Convert.ToInt32(ddlTeachers.SelectedValue);
            FillTeacherDetails();
            FillClasses();
        }
    }
}