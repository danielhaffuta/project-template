﻿using Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace ReshitScheduler
{
    public partial class ClassPage :BasePage
    {

        private DataTable dtDays;
        private DataTable dtHours;
        private DataTable dtCourses;
        private DataTable dtScheduleTable;


        private string strClassID;
        protected string strClassName;
        private static string strPreviousPage;
        public static Teacher LoggedInTeacher;


        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["CurrentYearID"] == null)
            {
                Session["CurrentYearID"] = nYearId;
            }
            if (Session["LoggedInTeacher"] == null)
            {
                //Response.Redirect("LoginForm.aspx");
                //return;
            }
            else
            {
                LoggedInTeacher = Session["LoggedInTeacher"] as Teacher;
            }
            /*if (Request.QueryString["ClassID"] == null /* Session["ClassID"] == null)
            {
                Session["ClassID"] = 1;
                //Response.Redirect("MainForm.aspx");
                //return;
            }*/
            if (!IsPostBack)
            {
                strPreviousPage = Request.UrlReferrer?.ToString() ?? "LoginForm.aspx";

            }
            LoadClassSchedule();
            FillStudents();
        }

        private void LoadClassSchedule()
        {
            strClassID = Request.QueryString["ClassID"]?.ToString() ?? "5";
            strClassName = DBConnection.Instance.GetConstraintData("classes", Convert.ToInt32(strClassID));
            dtDays = DBConnection.Instance.GetAllDataFromTable("days", string.Empty);
            dtHours = DBConnection.Instance.GetConstraintDataTable("hours_in_day", "where hours_in_day.year_id = " + nYearId, "order by hour_of_school_day");
            dtCourses = DBConnection.Instance.GetConstraintDataTable("courses");

            BuildEmptySchedule();
            FillSchedule();
            FillAndAddGrid();

        }

        private void BuildEmptySchedule()
        {
            dtScheduleTable = new DataTable("Schedule");
            dtScheduleTable.Columns.Add("hour_id", typeof(string));
            dtScheduleTable.Columns.Add("days_hours", typeof(string));

            //Setting days columns
            foreach (DataRow drCurrentDay in dtDays.Rows)
            {
                dtScheduleTable.Columns.Add(drCurrentDay["id"].ToString(), typeof(string));
            }
            ///////////////////////////////

            //Setting days Row
            DataRow drNewRow = dtScheduleTable.NewRow();
            drNewRow["days_hours"] = "ימים/שעות";
            foreach (DataRow drCurrentDay in dtDays.Rows)
            {
                drNewRow[drCurrentDay["id"].ToString()] = drCurrentDay["day_name"].ToString();
                
            }
            dtScheduleTable.Rows.Add(drNewRow);
            ////////////////////////////////////////////////////////

            //Setting Hours Rows
            foreach (DataRow drCurrentHour in dtHours.Rows)
            {
                drNewRow = dtScheduleTable.NewRow();
                drNewRow["hour_id"] = drCurrentHour["id"].ToString();
                drNewRow["days_hours"] = drCurrentHour["name"].ToString();
                if (drCurrentHour["is_break"].ToString().Equals("1"))
                {
                    drNewRow["hour_id"] += "*";
                }
                dtScheduleTable.Rows.Add(drNewRow);

            }
            //////////////////////////////////////////////////
        }

        private void FillSchedule()
        {
            DataTable dtClassSchedule = DBConnection.Instance.GetDataTableByQuery(
                "select * from classes_schedule where class_id = " + strClassID);
            DataTable dtStudentsSchedule = DBConnection.Instance.GetDataTableByQuery(
            "SELECT * FROM students_schedule " +
            //            " inner join students on students.id = students_schedule.student_id" +
            " inner join students_classes on students_classes.student_id = students_schedule.student_id" +
            " inner join classes on classes.id = students_classes.class_id" +
            " inner join teachers on teachers.id = classes.teacher_id and teachers.year_id = " + nYearId +
            " where students_classes.class_id = " + strClassID);
            foreach (DataRow drCurrentHour in dtClassSchedule.Rows)
            {
                string strCourseName = dtCourses.Select("id = " + drCurrentHour["course_id"].ToString())[0]["name"].ToString() +
                                       "*" + drCurrentHour["hour_id"] + "-" + drCurrentHour["day_id"];
                DataRow[] drStudentScheduleRows = dtStudentsSchedule.Select("hour_id = " + drCurrentHour["hour_id"].ToString() + " and day_id = " + drCurrentHour["day_id"].ToString());
                if (drStudentScheduleRows.Count() > 0)
                {

                    strCourseName += "$";
                }
                dtScheduleTable.Select("hour_id = '" + drCurrentHour["hour_id"].ToString().Replace("*","")+"'")[0][drCurrentHour["day_id"].ToString()] = strCourseName;
            }

            

        }

        private void FillAndAddGrid()
        {
            GridView gvScheduleView = new GridView()
            {
                ShowHeader = false,
                CssClass = "table table-bordered table-sm",
                DataSource = dtScheduleTable
            };
            gvScheduleView.DataBind();

            foreach (GridViewRow gvrCurrentRow in gvScheduleView.Rows)
            {
                if (gvrCurrentRow.Cells[0].Text.Contains("*"))
                {
                    gvrCurrentRow.Cells[0].Text.Replace("*", "");
                    gvrCurrentRow.Cells[1].HorizontalAlign = HorizontalAlign.Center;
                    gvrCurrentRow.CssClass = "bg-light";
                    gvrCurrentRow.Cells[2].ColumnSpan = 6;
                    gvrCurrentRow.Cells[2].Text = "הפסקה";
                    gvrCurrentRow.Cells[2].CssClass = "h5 text-center";
                    
                    gvrCurrentRow.Cells.RemoveAt(7);
                    gvrCurrentRow.Cells.RemoveAt(6);
                    gvrCurrentRow.Cells.RemoveAt(5);
                    gvrCurrentRow.Cells.RemoveAt(4);
                    gvrCurrentRow.Cells.RemoveAt(3);
                }
                else
                {
                    foreach (TableCell tcCurrentCell in gvrCurrentRow.Cells)
                    {
                        tcCurrentCell.Text = tcCurrentCell.Text.Replace("&lt;br&gt;", "<br>");
                        tcCurrentCell.HorizontalAlign = HorizontalAlign.Center;
                        if (tcCurrentCell.Text.Contains("*"))
                        {
                            Label lbl = new Label() { Text = tcCurrentCell.Text.Split('*')[0] };
                            tcCurrentCell.Attributes.Add("onClick", "OnClick(\"GroupsForm.aspx?IDs=" + strClassID + "-" + tcCurrentCell.Text.Split('*')[1].Replace("$", "") + "\",1200,900,\"yes\");");
                            tcCurrentCell.Controls.Add(lbl);

                            if (tcCurrentCell.Text.Contains("$"))
                            {
                                //tcCurrentCell.Text = tcCurrentCell.Text.Replace("$", "");
                                LinkButton lbGroupLinkButton = new LinkButton()
                                {
                                    Text = "<br>קבוצות",
                                    ID = tcCurrentCell.Text.Split('*')[1],
                                    OnClientClick = "genericPopup(\"GroupsForm.aspx?IDs=" + strClassID + "-" + tcCurrentCell.Text.Split('*')[1].Replace("$", "") + "\",1200,900,\"yes\")"
                                };
                                tcCurrentCell.Controls.Add(lbGroupLinkButton);
                            }
                        }
                    }
                }
                gvrCurrentRow.Cells[0].Visible = false;
            }
            pnlSchedule.Controls.Add(gvScheduleView);
        }

        private void FillStudents()
        {
            DataTable dtStudents = DBConnection.Instance.GetDataTableByQuery("select students.id,concat(first_name,' ' ,last_name) as name,picture_path from students" +
                                                                            " inner join students_classes on students_classes.student_id = students.id" +
                                                                            " where students_classes.class_id = " + strClassID);

            foreach (DataRow drCurrentStudent in dtStudents.Rows)
            {
                string strFigure = "<a href=\"StudentDetailsForm.aspx?StudentID="+ drCurrentStudent["id"] + "\" class=\"col-12 col-md-2 col-sm-4\">"+
                                   "<figure > <img src=\"" + drCurrentStudent["picture_path"] + "\" width=\"100\">"+
                                   "<figcaption>" + drCurrentStudent["name"] + "</figcaption></figure></a>";
                pnlStudents.Controls.Add(new LiteralControl(strFigure));
            }
        }

        protected void BtnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("MainForm.aspx");
            //GoBack();
        }
        protected void BtnLogout_Click(object sender, EventArgs e)
        {
            Response.Redirect("LoginForm.aspx");
            //GoBack();
        }

        protected void GotoCoursesAndGroupsForm(object sender, EventArgs e)
        {
            Response.Redirect("TeacherCoursesAndGroupsForm.aspx?TeacherID=" + LoggedInTeacher.Id + "&ClassID=" + strClassID);

        }

        private void GoBack()
        {
            Response.Redirect(strPreviousPage);
        }
    }
}