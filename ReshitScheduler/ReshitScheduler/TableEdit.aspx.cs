﻿using Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ReshitScheduler
{
    public partial class TableEdit : System.Web.UI.Page
    {
        public static string strTableName;
        public static string strWhereClause;

        //public static Teacher LoggedInTeacher;

        protected void Page_PreInit(object sender, EventArgs e)
        {
            strWhereClause = string.Empty;
            if (Request.QueryString["IDs"] != null)
            {

                string[] strIDs = Request.QueryString["IDs"].Split('-');
                strWhereClause = " where student_id in (select id from students where class_id in (select id from classes where id = " + strIDs[0] + "))" +
                                 " and hour_id = " + strIDs[1] +
                                 " and day_id = " + strIDs[2] +
                                 " and group_id = " + strIDs[3];
            }
            if (Session["LoggedInTeacher"] == null)
            {
                //Response.Redirect("LoginForm.aspx");
                //return;
            }
            if (strTableName == null)
            {
                strTableName = "classes";
            }
            this.FillGridAndAddControls();
        }

        protected void Page_Init(object sender, EventArgs e)
        {
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["TableGrid"] = TableGrid;
            }

        }
        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            Session["TableGrid"] = TableGrid;
        }

        private void FillGridAndAddControls()
        {
            DataTable dtTable = DBConnection.Instance.GetAllDataFromTable(strTableName, strWhereClause);
            FillGridIfEmpty(ref dtTable);
            TableGrid.EditIndex = Convert.ToInt32((Session["TableGrid"] as GridView)?.EditIndex + 1) - 1;
            TableGrid.DataSource = dtTable;
            TableGrid.DataKeyNames = dtTable.PrimaryKey.Select(pk => pk.ColumnName).ToArray();
            this.DataBind();
        }

        public override void DataBind()
        {
            TableGrid.DataBind();
            AddFooter();
            ReplaceForeignKeys();

        }

        private void AddFooter()
        {
            Button btnAdd = new Button() { Text = "Add", OnClientClick = "BtnAdd_Click" };
            btnAdd.Click += BtnAdd_Click;
            TableGrid.FooterRow.Cells[1].Controls.Add(btnAdd);
            for (int nCurrentCell = 2; nCurrentCell < TableGrid.FooterRow.Cells.Count; nCurrentCell++)
            {
                TextBox txtCellTextBox = new TextBox()
                {
                    ID = "footer_" + ((TableGrid.FooterRow.Cells[nCurrentCell] as DataControlFieldCell).ContainingField as AutoGeneratedField).DataField
                };
                TableGrid.FooterRow.Cells[nCurrentCell].Controls.Add(txtCellTextBox);
            }
        }

        private void FillGridIfEmpty(ref DataTable dtTable)
        {
            if (dtTable.Rows.Count == 0) // the table is empty 
            {
                dtTable = DBConnection.Instance.GetEmptyDataTable(dtTable);
            }
        }


        private int FindKeyIndex(string strKeyName, GridViewRow gvrHeaderRow)
        {
            for (int nCurrentColumn = 0; nCurrentColumn < gvrHeaderRow.Cells.Count; nCurrentColumn++)
            {
                if (gvrHeaderRow.Cells[nCurrentColumn].Text == strKeyName)
                {
                    return nCurrentColumn;

                }
            }
            return -1;
        }

        private DropDownList GetDropDownList(DataTable dtDataSource, string strID, string strSelectedID)
        {
            DropDownList ddlData = new DropDownList()
            {
                DataSource = dtDataSource,
                DataValueField = "id",
                AutoPostBack = true,
                DataTextField = "name",
                ID = strID,

            };

            ddlData.DataBind();
            foreach (ListItem liCurrentItem in ddlData.Items)
            {
                liCurrentItem.Text = liCurrentItem.Text.Replace("<br>", "");
            }

            ddlData.SelectedIndex = FindIndexById(ddlData, strSelectedID);
            ddlData.SelectedIndexChanged += DdlData_SelectedIndexChanged;
            return ddlData;
        }

        private DropDownList GetDropDownList(DataTable dtDataSource, string strID)
        {
            DropDownList ddlData = new DropDownList()
            {
                DataSource = dtDataSource,
                DataValueField = "id",
                AutoPostBack = true,
                DataTextField = "name",
                ID = strID,

            };

            ddlData.DataBind();
            ddlData.SelectedIndex = 0;
            ddlData.SelectedIndexChanged += DdlData_SelectedIndexChanged;
            return ddlData;
        }

        private void ReplaceForeignKeyInRow(GridViewRow gvrRow, DataTable dtKeyData, string strKeyTableName, int nColumnIndex)
        {
            DropDownList ddlData;

            string strID = "DDL_" + strKeyTableName + "_" + gvrRow.RowIndex;
            if (TableGrid.EditIndex == gvrRow.RowIndex &&
                !TableGrid.DataKeyNames.Contains((gvrRow.Cells[nColumnIndex] as DataControlFieldCell).ContainingField.HeaderText))
            {

                ddlData = GetDropDownList(dtKeyData, strID, (gvrRow.Cells[nColumnIndex].Controls[0] as TextBox).Text);
                gvrRow.Cells[nColumnIndex].Controls[0].Visible = false;

                ddlData.Enabled = true;
            }
            else
            {
                ddlData = GetDropDownList(dtKeyData, strID, gvrRow.Cells[nColumnIndex].Text);
                ddlData.Enabled = false;

            }
            gvrRow.Cells[nColumnIndex].Controls.Add(ddlData);

        }

        private void ReplaceForeignKey(string strKeyTableName, int nColumnIndex)
        {
            DataTable dtKeyData = DBConnection.Instance.GetConstraintDataTable(strKeyTableName);
            DropDownList ddlData;
            foreach (GridViewRow dvrCurrentRow in TableGrid.Rows)
            {
                ReplaceForeignKeyInRow(dvrCurrentRow, dtKeyData, strKeyTableName, nColumnIndex);
            }

            /**
             * Footer
             */
            string strID = "DDL_" + strKeyTableName + "_insert_footer";
            ddlData = GetDropDownList(dtKeyData, strID);
            foreach (ListItem liCurrentItem in ddlData.Items)
            {
                liCurrentItem.Text = liCurrentItem.Text.Replace("<br>", "");
            }
            TableGrid.FooterRow.Cells[nColumnIndex].Controls.Add(ddlData);
            ddlData.SelectedIndex = 0;
            TableGrid.FooterRow.Cells[nColumnIndex].Controls[0].Visible = false;
            ((TextBox)TableGrid.FooterRow.Cells[nColumnIndex].Controls[0]).Text = ddlData.SelectedValue;

        }

        private void ReplaceForeignKeys()
        {
            DBConnection dbcConnection = DBConnection.Instance;
            //GridView gvTableGrid = Session["TableGrid"] as GridView ?? TableGrid;
            DataTable dtDataSource = TableGrid.DataSource as DataTable;
            DataTable dtForeignKeys = dbcConnection.GetForeignKeys(dtDataSource.TableName);

            foreach (DataRow drCurrentKey in dtForeignKeys.Rows)
            {
                int nColumnIndex = FindKeyIndex(drCurrentKey[0].ToString(), TableGrid.HeaderRow);
                ReplaceForeignKey(drCurrentKey[1].ToString(), nColumnIndex);
            }
        }

        private void ReplaceForeignKeysInEditRow()
        {

            DBConnection dbcConnection = DBConnection.Instance;
            GridView gvTableGrid = Session["TableGrid"] as GridView ?? TableGrid;
            if (TableGrid.EditIndex == -1)
                return;

            DataTable dtDataSource = gvTableGrid.DataSource as DataTable;
            DataTable dtForeignKeys = dbcConnection.GetForeignKeys(dtDataSource.TableName);

            foreach (DataRow drCurrentKey in dtForeignKeys.Rows)
            {
                int nColumnIndex = FindKeyIndex(drCurrentKey[0].ToString(), gvTableGrid.HeaderRow);
                DataTable dtKeyData = DBConnection.Instance.GetConstraintDataTable(drCurrentKey[1].ToString());

                ReplaceForeignKeyInRow(TableGrid.Rows[TableGrid.EditIndex], dtKeyData, drCurrentKey[1].ToString(), nColumnIndex);
                TableGrid.Rows[TableGrid.EditIndex].Cells[nColumnIndex].Controls[0].Visible = false;
            }
        }

        private int FindIndexById(DropDownList ddlList, string strID)
        {
            for (int nCurrentItemIndex = 0; nCurrentItemIndex < ddlList.Items.Count; nCurrentItemIndex++)
            {
                if (ddlList.Items[nCurrentItemIndex].Value == strID)
                {
                    return nCurrentItemIndex;
                }
            }
            return 0;
        }


        public void DdlData_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlSender = (DropDownList)sender;
            ((TextBox)ddlSender.Parent.Controls[0]).Text = ddlSender.SelectedValue;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            GridViewRow row = TableGrid.FooterRow;
            DBConnection dbcConnection = DBConnection.Instance;
            if (!dbcConnection.InsertTableRow(TableGrid.DataSource as DataTable, row))
            {
                Helper.ShowMessage(ClientScript, GetType(), "error saving");
            }
            TableGrid.EditIndex = -1;
            TableGrid.DataSource = DBConnection.Instance.GetAllDataFromTable(strTableName, strWhereClause);
            DataBind();
        }

        protected void TableGrid_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridViewRow row = TableGrid.Rows[e.RowIndex];
            int nId = Convert.ToInt32(TableGrid.DataKeys[e.RowIndex].Values[0]);
            DBConnection dbcConnection = DBConnection.Instance;
            if (!dbcConnection.UpdateTableRow(strTableName, nId, row))
            {
                Helper.ShowMessage(ClientScript, GetType(), "error saving");
            }
            TableGrid.EditIndex = -1;
            TableGrid.DataSource = DBConnection.Instance.GetAllDataFromTable(strTableName, strWhereClause);
            this.DataBind();
        }

        protected void TableGrid_RowEditing(object sender, GridViewEditEventArgs e)
        {
            TableGrid.EditIndex = e.NewEditIndex;
            this.DataBind();
        }

        protected void TableGrid_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            TableGrid.EditIndex = -1;
            this.DataBind();
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdminForm.aspx");
        }

    }
}