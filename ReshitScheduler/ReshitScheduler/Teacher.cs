﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReshitScheduler
{
    public class Teacher
    {
        private int nID;
        private int nClassID;
        private string strFirstName;
        private string strLastName;
        private string strType;

        public int ID
        {
            get
            {
                return nID;
            }

            set
            {
                nID = value;
            }
        }

        public int ClassID
        {
            get
            {
                return nClassID;
            }

            set
            {
                nClassID = value;
            }
        }

        public string FirstName
        {
            get
            {
                return strFirstName;
            }

            set
            {
                strFirstName = value;
            }
        }

        public string LastName
        {
            get
            {
                return strLastName;
            }

            set
            {
                strLastName = value;
            }
        }

        public string Type
        {
            get
            {
                return strType;
            }

            set
            {
                strType = value;
            }
        }
    }
}