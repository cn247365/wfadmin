using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VHSSyncSAPToWorkflow
{
    public class VendorData
    {
        private string strVendorCode;
        public string VendorCode
        {
            get { return strVendorCode; }
            set { strVendorCode = value; }
        }

        private string strName_EN;

        public string Name_EN
        {
            get { return strName_EN; }
            set { strName_EN = value; }
        }


        private string strName_CN;

        public string Name_CN
        {
            get { return strName_CN; }
            set { strName_CN = value; }
        }

        private string strBank;

        public string Bank
        {
            get { return strBank; }
            set { strBank = value; }
        }

        private string strBankAccount;

        public string BankAccount
        {
            get { return strBankAccount; }
            set { strBankAccount = value; }
        }

        private string strPaymentTerms;

        public string PaymentTerms
        {
            get { return strPaymentTerms; }
            set { strPaymentTerms = value; }
        }

        private string strContactPerson;

        public string ContactPerson
        {
            get { return strContactPerson; }
            set { strContactPerson = value; }
        }

        private string strEmail;

        public string Email
        {
            get { return strEmail; }
            set { strEmail = value; }
        }

        private string strAddress;

        public string Address
        {
            get { return strAddress; }
            set { strAddress = value; }
        }

        private string strAddressCN;

        public string AddressCN
        {
            get { return strAddressCN; }
            set { strAddressCN = value; }
        }

        private string strTel;

        public string Tel
        {
            get { return strTel; }
            set { strTel = value; }
        }

        private string strTel2;

        public string Tel2
        {
            get { return strTel2; }
            set { strTel2 = value; }
        }

        private string strCompanyCode;

        public string CompanyCode
        {
            get { return strCompanyCode; }
            set { strCompanyCode = value; }
        }

        private string strIsInvalued;

        public string IsInvalued
        {
            get { return strIsInvalued; }
            set { strIsInvalued = value; }
        }


    }
}
