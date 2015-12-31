using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace App1
{
    class XMLParserClass
    {
        private XElement XMLDocElements;
        
        public XMLParserClass(string XMLDocumentName)
        {
            try
            {
                XMLDocElements = XElement.Load(XMLDocumentName);
            }
            catch(Exception e)
            {
                
            }
            
        }

        public void SetSourceFile(string FileName)
        {
            try
            {
                XMLDocElements = XElement.Load(FileName);
            }
            catch (Exception e)
            {

            }
        }

        public void ReadWx(string RegionName)
        {
            IEnumerable<XElement> LocationNode =
                    from EnumLocation in XMLDocElements.Elements("location")
                    select EnumLocation;

            IEnumerable<XElement> Region =
                    from EnumRegion in LocationNode.Descendants() 
                    where (string)EnumRegion.Attribute("Region") == RegionName
                    select EnumRegion;

            //string RegionName = EnumRegion.Value;
            
        }

        public void ReadCitiesInProvince(string ProvinceName, List<string> CityNameList)
        {
            if(XMLDocElements != null)
            {
                IEnumerable<XElement> ProvinceNode =
                    from EnumProv in XMLDocElements.Elements("Province")
                    where (string)EnumProv.Attribute("Name") == ProvinceName
                    select EnumProv;

                IEnumerable<XElement> Cities =
                    from EnumCity in ProvinceNode.Descendants()
                    select EnumCity;

                foreach (XElement EnumCity in Cities)
                {

                    string CityName = EnumCity.Value;
                    CityNameList.Add(CityName);
                    CityNameList.Sort();
                }
            }
            
        }

        //public void ReadCitiesInProduct(string ProductName, List<string> CityNameList)
        //{
        //    if (XMLDocElements != null)
        //    {
        //        IEnumerable<XElement> ProductNode =
        //            from EnumProv in XMLDocElements.Elements("Product")
        //            where (string)EnumProv.Attribute("Name") == ProductName
        //            select EnumProv;

        //        IEnumerable<XElement> Cities =
        //            from EnumCity in ProductNode.Descendants()
        //            select EnumCity;

        //        foreach (XElement EnumCity in Cities)
        //        {

        //            string CityName = EnumCity.Value;
        //            CityNameList.Add(CityName);
        //            CityNameList.Sort();
        //        }
        //    }

        //}

        //public List<string> ReadProductList()
        //{
        //    List<string> ProductList = null;

        //    if (XMLDocElements != null)
        //    {
        //        ProductList = new List<string>();

        //        IEnumerable<XElement> ProductNode =
        //            from EnumProd in XMLDocElements.Elements("Product")
        //            select EnumProd;

        //        foreach (XElement Product in ProductNode)
        //        {
        //            string ProductName = Product.FirstAttribute.Value;
        //            ProductList.Add(ProductName);
        //        }

        //        ProductList.Sort();
        //    }

        //    return ProductList; 
        //}
        //public void ReadProductOptionList(string ProductName, List<string> OptionList)
        //{
        //    if (XMLDocElements != null)
        //    {
        //        IEnumerable<XElement> ProductNode =
        //            from EnumProv in XMLDocElements.Elements("Product")
        //            where (string)EnumProv.Attribute("Name") == ProductName
        //            select EnumProv;

        //        IEnumerable<XElement> Options =
        //            from EnumOptions in ProductNode.Descendants()
        //            where EnumOptions.Name == "Option"
        //            select EnumOptions;

        //        foreach (XElement EnumOption in Options)
        //        {

        //            string Option = EnumOption.FirstAttribute.Value;
        //            OptionList.Add(Option);
        //        }

        //        OptionList.Sort();
        //    }

        //}

        //This is used to read the list of Option2 "choices"
        //public void ReadChoiceList(string ProductName, string Option, List<string> ChoiceList)
        //{
        //    if (XMLDocElements != null)
        //    {
        //        IEnumerable<XElement> ProductNode =
        //            from EnumProv in XMLDocElements.Elements("Product")
        //            where (string)EnumProv.Attribute("Name") == ProductName
        //            select EnumProv;

        //        IEnumerable<XElement> Options =
        //            from EnumOptions in ProductNode.Descendants()
        //            where (string)EnumOptions.Attribute("Name") == Option
        //            select EnumOptions;

        //        IEnumerable<XElement> Choices =
        //            from EnumChoice in Options.Descendants()
        //            where EnumChoice.Name == "Option2"
        //            select EnumChoice;

        //        foreach (XElement EnumChoice in Choices)
        //            ChoiceList.Add(EnumChoice.FirstAttribute.Value);

        //        ChoiceList.Sort();
        //    }

        //}

        //For use only with weather camera data
        //public void ReadDirList(string ProductName, string RegionName, string StationName, List<string> DirList)
        //{
        //    if (XMLDocElements != null)
        //    {
        //        IEnumerable<XElement> ProductNode =
        //            from EnumProv in XMLDocElements.Elements("Product")
        //            where (string)EnumProv.Attribute("Name") == ProductName
        //            select EnumProv;

        //        IEnumerable<XElement> Options =
        //            from EnumOptions in ProductNode.Descendants()
        //            where (string)EnumOptions.Attribute("Name") == RegionName
        //            select EnumOptions;

        //        if (StationName != null)
        //        {
        //            IEnumerable<XElement> Choices =
        //            from EnumChoice in Options.Descendants()
        //            where (string)EnumChoice.Attribute("Name") == StationName
        //            select EnumChoice;

        //            IEnumerable<XElement> Dirs =
        //            from EnumDir in Choices.Descendants()
        //            where EnumDir.Name == "Dir"
        //            select EnumDir;

        //            IEnumerable<XElement> Code =
        //            from EnumCode in Choices.Descendants()
        //            where EnumCode.Name == "Code"
        //            select EnumCode;

        //            DirList.AddRange(Dirs.First().Value.Split(','));
        //            DirList.Sort();
        //        }
        //    }
        //}

        //public List<string> ReadProvinceList()
        //{
        //    List<string> ProvinceList = new List<string>();
        //    if (XMLDocElements != null)
        //    {
        //        IEnumerable<XElement> Provinces =
        //            from EnumProv in XMLDocElements.Elements("Province")
        //            select EnumProv;

        //        IEnumerable<XAttribute> attList =
        //            from at in Provinces.Attributes()
        //            select at;

        //        foreach (XAttribute EnumProv in attList)
        //        {
        //            ProvinceList.Add(EnumProv.Value);
        //        }
        //    }

        //    return ProvinceList;
        //}

        //public string ReadBaseURL(string ProductName)
        //{
        //    if (XMLDocElements != null)
        //    {
        //        IEnumerable<XElement> ProductNode =
        //            from EnumProv in XMLDocElements.Elements("Product")
        //            where (string)EnumProv.Attribute("Name") == ProductName
        //            select EnumProv;

        //        IEnumerable<XElement> Options =
        //            from EnumOptions in ProductNode.Descendants()
        //            where EnumOptions.Name.ToString().Equals("BaseURL")
        //            select EnumOptions;

        //        return Options.First().Value.ToString();
        //    }

        //    return null;

        //}

        //public string ReadCode(string ProductName, string RegionName, string StationName)
        //{
        //    IEnumerable<XElement> Choices = null;

        //    if (XMLDocElements != null)
        //    {
        //        IEnumerable<XElement> ProductNode =
        //            from EnumProv in XMLDocElements.Elements("Product")
        //            where (string)EnumProv.Attribute("Name") == ProductName
        //            select EnumProv;

        //        IEnumerable<XElement> Options =
        //            from EnumOptions in ProductNode.Descendants()
        //            where (string)EnumOptions.Attribute("Name") == RegionName
        //            select EnumOptions;

        //        if(StationName != null)
        //        {
        //            Choices =
        //            from EnumChoice in Options.Descendants()
        //            where (string)EnumChoice.Attribute("Name") == StationName
        //            select EnumChoice;

        //            IEnumerable<XElement> Codes =
        //            from EnumOptions in Choices.Descendants()
        //            where EnumOptions.Name.ToString().Equals("Code")
        //            select EnumOptions;
        //            if(Codes.Count() > 0)
        //                return Codes.First().Value.ToString();
        //        }
        //        else
        //            return Options.First().Value.ToString();
        //    }

        //    return "";

        //}

        //public string ReadNodeField(string ProductName, string Region, string Station, string Dir, string Field)
        //{
        //    if (XMLDocElements != null)
        //    {
        //        IEnumerable<XElement> ProductNode =
        //            from EnumProv in XMLDocElements.Elements("Product")
        //            where (string)EnumProv.Attribute("Name") == ProductName
        //            select EnumProv;

        //        IEnumerable<XElement> Options =
        //            from EnumOptions in ProductNode.Descendants()
        //            where EnumOptions.Name.ToString().Equals(Field)
        //            select EnumOptions;

        //        return Options.First().Value.ToString();
        //    }

        //    return "";

        //}

        //public string GetCityCode(string CityName)
        //{
        //    XElement City, CityCode;
            
        //    if (XMLDocElements != null)
        //    {
        //        IEnumerable<XElement> CityNode =
        //            from EnumProv in XMLDocElements.Elements("City")
        //            where (string)EnumProv.Attribute("Name") == CityName
        //            select EnumProv;

        //        City = CityNode.First();
        //        CityCode = (XElement)City.FirstNode;
                                                
        //        return CityCode.Value.ToString();
        //    }

        //    return "Failed";
        //}

        //public string GetHomeURL(string CityName)
        //{
        //    XElement City, BaseURL;

        //    if (XMLDocElements != null)
        //    {
        //        IEnumerable<XElement> CityNode =
        //            from EnumProv in XMLDocElements.Elements("City")
        //            where (string)EnumProv.Attribute("Name") == CityName
        //            select EnumProv;

        //        City = CityNode.First();
        //        BaseURL = (XElement)City.LastNode;

        //        return BaseURL.Value.ToString();
        //    }

        //    return "Failed";
        //}


    }
}
