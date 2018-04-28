﻿using Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ReshitScheduler
{
    public partial class AdminForm : System.Web.UI.Page
    {
        public static Teacher LoggedInTeacher;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["LoggedInTeacher"] != null)
            {
                LoggedInTeacher = Session["LoggedInTeacher"] as Teacher;
                AdminName.Text = LoggedInTeacher.FirstName + " " + LoggedInTeacher.LastName;
                PopulateMenu();
                Button btnLogout = new Button() { Text = "Logout" };
                btnLogout.Click += BtnLogout_Click;
                form1.Controls.Add(btnLogout);
            }
            else
            {
                Response.Redirect("LoginForm.aspx");
                return;
            }

        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            Session["LoggedInTeacher"] = null;
            Response.Redirect("LoginForm.aspx");
            return;
        }

        private void PopulateMenu()
        {
            DataTable dtTables = DBConnection.Instance().GetDataTableByQuery("select table_name from INFORMATION_SCHEMA.tables where table_schema = 'reshit'");
            string tableName = "";
            string tableDisplayName="";
            List<ListItem> items = new List<ListItem>();
            items.Add(new ListItem(tableDisplayName, tableName));
            foreach (DataRow CurrentTable in dtTables.Rows)
            {
                tableName = (string)CurrentTable["table_name"];
                tableDisplayName = DBConnection.Instance().GetStringByQuery("select hebrew_name from tables_information where table_name ='"+tableName+"'");
                items.Add(new ListItem(tableDisplayName, tableName));
               
            }
            courseEdit.Items.AddRange(items.ToArray());
        }

        protected void itemSelected(object sender, EventArgs e)
        {
            DropDownList dropDown = (DropDownList)sender;
            TableEdit.strTableName = dropDown.SelectedValue;
            Response.Redirect("TableEdit.aspx");

        }
    }
}