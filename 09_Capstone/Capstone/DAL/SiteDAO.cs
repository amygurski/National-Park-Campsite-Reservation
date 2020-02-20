﻿using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Capstone.DAL
{
    public class SiteDAO : ISiteDAO
    {
        private string connectionString;

        public SiteDAO(string connString)
        {
            this.connectionString = connString;
        }

        public List<Site> GetSites()
        {
            List<Site> list = new List<Site>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {

                    conn.Open();

                    // Create the command for the sql statement
                    string sql = "Select * from site";
                    SqlCommand cmd = new SqlCommand(sql, conn);

                    // Execute the query and get the result set in a reader
                    SqlDataReader rdr = cmd.ExecuteReader();

                    // For each row, create a new country and add it to the list
                    while (rdr.Read())
                    {
                        list.Add(RowToObject(rdr));
                    }

                }
            }
            catch (SqlException ex)
            {
                // TODO: Add exception log
                throw;
            }

            return list;
        }

        public bool hasAvailableSites(int campgroundId, DateTime arrivalDate, DateTime departureDate)
        {
            List<Site> list = GetTop5AvailableSites(campgroundId, arrivalDate, departureDate);

            if (list.Count == 0)
            {
                return false; //no sites, need to prompt user if they want a different date
            }
            else
            {
                return true; //yay, there are sites
            }
        }
        
        public List<Site> GetTop5AvailableSites(int campgroundId, DateTime arrivalDate, DateTime departureDate)
        {
            List<Site> list = new List<Site>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {

                    conn.Open();

                    // Sql command gets any conflicting dates
                    string sql = "SELECT DISTINCT TOP 5 s.*, daily_fee * DATEDIFF(Day, '2020-02-24', '2020-03-01') as total_fee" +
                    "FROM site s" +
                    "LEFT JOIN reservation r ON s.site_id = r.site_id" +
                    "JOIN campground c ON s.campground_id = c.campground_id" +
                    "WHERE c.campground_id = @campgroundId" +
                    "AND((@arrivalDate NOT BETWEEN from_date AND to_date) AND(@departureDate NOT BETWEEN from_date AND to_date))";

                    SqlCommand cmd = new SqlCommand(sql, conn);

                    cmd.Parameters.AddWithValue("@campgroundId", campgroundId);
                    cmd.Parameters.AddWithValue("@arrivalDate", arrivalDate);
                    cmd.Parameters.AddWithValue("@departureDate", departureDate);

                    // Execute the query and get the result set in a reader
                    SqlDataReader rdr = cmd.ExecuteReader();

                    // For each row, create a site and add it to the list
                    while (rdr.Read())
                    {
                        list.Add(RowToObject(rdr));
                    }

                    return list;
                }
            }
            catch (SqlException ex)
            {
                // TODO: Add exception log
                throw;
            }
        }

        private static Site RowToObject(SqlDataReader rdr)
        {
            Site site = new Site()
            {
                SiteId = Convert.ToInt32(rdr["site_id"]),
                CampgroundId = Convert.ToInt32(rdr["campground_id"]),
                SiteNumber = Convert.ToInt32(rdr["site_number"]),
                MaxOccupancy = Convert.ToInt32(rdr["max_occupancy"]),
                IsAccessible = Convert.ToBoolean(rdr["accessible"]),
                MaxRVLength = Convert.ToInt32(rdr["max_rv_length"]),
                HasUtilities = Convert.ToBoolean(rdr["utilities"])
            };

            return site;
        }

    }
}
