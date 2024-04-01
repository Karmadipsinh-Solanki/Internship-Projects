--
-- PostgreSQL database dump
--

-- Dumped from database version 16.1
-- Dumped by pg_dump version 16.1

-- Started on 2024-03-28 19:21:59

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 4 (class 2615 OID 2200)
-- Name: public; Type: SCHEMA; Schema: -; Owner: pg_database_owner
--

CREATE SCHEMA public;


ALTER SCHEMA public OWNER TO pg_database_owner;

--
-- TOC entry 5179 (class 0 OID 0)
-- Dependencies: 4
-- Name: SCHEMA public; Type: COMMENT; Schema: -; Owner: pg_database_owner
--

COMMENT ON SCHEMA public IS 'standard public schema';


--
-- TOC entry 287 (class 1255 OID 40960)
-- Name: patientdashboarddata(integer); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.patientdashboarddata(id integer) RETURNS TABLE(requestid integer, createddate timestamp without time zone, status smallint, name text, count bigint)
    LANGUAGE plpgsql
    AS $_$
BEGIN 
    RETURN QUERY
    select r."RequestId",r."CreatedDate", r."Status",CONCAT(p."FirstName",' ',p."LastName") as "PhysicianName",(select count(*) from "RequestWiseFile" group by "RequestId" having "RequestId" = r."RequestId") as Count from "Request" as r inner join "RequestStatusLog" as rsl on r."RequestId" = rsl."RequestId" left join "Physician" as p on r."PhysicianId" = p."PhysicianId" where r."UserId" = $1;
END; 
$_$;


ALTER FUNCTION public.patientdashboarddata(id integer) OWNER TO postgres;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 220 (class 1259 OID 32791)
-- Name: Admin; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Admin" (
    "AdminId" integer NOT NULL,
    "AspNetUserId" integer NOT NULL,
    "FirstName" character varying(100) NOT NULL,
    "LastName" character varying(100),
    "Email" character varying(50) NOT NULL,
    "Mobile" character varying(20),
    "Address1" character varying(500),
    "Address2" character varying(500),
    "City" character varying(100),
    "RegionId" integer,
    "Zip" character varying(10),
    "AltPhone" character varying(20),
    "CreatedBy" integer NOT NULL,
    "CreatedDate" timestamp without time zone NOT NULL,
    "ModifiedBy" integer,
    "ModifiedDate" timestamp without time zone,
    "Status" smallint,
    "IsDeleted" boolean,
    "RoleId" integer
);


ALTER TABLE public."Admin" OWNER TO postgres;

--
-- TOC entry 263 (class 1259 OID 33078)
-- Name: AdminRegion; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."AdminRegion" (
    "AdminRegionId" integer NOT NULL,
    "AdminId" integer NOT NULL,
    "RegionId" integer NOT NULL
);


ALTER TABLE public."AdminRegion" OWNER TO postgres;

--
-- TOC entry 262 (class 1259 OID 33077)
-- Name: AdminRegion_AdminRegionId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."AdminRegion_AdminRegionId_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."AdminRegion_AdminRegionId_seq" OWNER TO postgres;

--
-- TOC entry 5180 (class 0 OID 0)
-- Dependencies: 262
-- Name: AdminRegion_AdminRegionId_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."AdminRegion_AdminRegionId_seq" OWNED BY public."AdminRegion"."AdminRegionId";


--
-- TOC entry 219 (class 1259 OID 32790)
-- Name: Admin_AdminId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Admin_AdminId_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."Admin_AdminId_seq" OWNER TO postgres;

--
-- TOC entry 5181 (class 0 OID 0)
-- Dependencies: 219
-- Name: Admin_AdminId_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Admin_AdminId_seq" OWNED BY public."Admin"."AdminId";


--
-- TOC entry 216 (class 1259 OID 32775)
-- Name: AspNetRoles; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."AspNetRoles" (
    "Id" integer NOT NULL,
    "Name" character varying(256) NOT NULL
);


ALTER TABLE public."AspNetRoles" OWNER TO postgres;

--
-- TOC entry 215 (class 1259 OID 32774)
-- Name: AspNetRoles_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."AspNetRoles_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."AspNetRoles_Id_seq" OWNER TO postgres;

--
-- TOC entry 5182 (class 0 OID 0)
-- Dependencies: 215
-- Name: AspNetRoles_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."AspNetRoles_Id_seq" OWNED BY public."AspNetRoles"."Id";


--
-- TOC entry 221 (class 1259 OID 32814)
-- Name: AspNetUserRoles; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."AspNetUserRoles" (
    "UserId" integer NOT NULL,
    "RoleId" integer NOT NULL
);


ALTER TABLE public."AspNetUserRoles" OWNER TO postgres;

--
-- TOC entry 218 (class 1259 OID 32782)
-- Name: AspNetUsers; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."AspNetUsers" (
    "Id" integer NOT NULL,
    "UserName" character varying(256) NOT NULL,
    "PasswordHash" text,
    "Email" character varying(256),
    "PhoneNumber" text,
    "IP" character varying(20),
    "CreatedDate" timestamp without time zone NOT NULL
);


ALTER TABLE public."AspNetUsers" OWNER TO postgres;

--
-- TOC entry 217 (class 1259 OID 32781)
-- Name: AspNetUsers_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."AspNetUsers_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."AspNetUsers_Id_seq" OWNER TO postgres;

--
-- TOC entry 5183 (class 0 OID 0)
-- Dependencies: 217
-- Name: AspNetUsers_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."AspNetUsers_Id_seq" OWNED BY public."AspNetUsers"."Id";


--
-- TOC entry 223 (class 1259 OID 32830)
-- Name: BlockRequests; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."BlockRequests" (
    "BlockRequestId" integer NOT NULL,
    "PhoneNumber" character varying(50),
    "Email" character varying(50),
    "IsActive" bit(1),
    "Reason" text,
    "RequestId" character varying(50) NOT NULL,
    "IP" character varying(20),
    "CreatedDate" timestamp without time zone,
    "ModifiedDate" timestamp without time zone
);


ALTER TABLE public."BlockRequests" OWNER TO postgres;

--
-- TOC entry 222 (class 1259 OID 32829)
-- Name: BlockRequests_BlockRequestId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."BlockRequests_BlockRequestId_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."BlockRequests_BlockRequestId_seq" OWNER TO postgres;

--
-- TOC entry 5184 (class 0 OID 0)
-- Dependencies: 222
-- Name: BlockRequests_BlockRequestId_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."BlockRequests_BlockRequestId_seq" OWNED BY public."BlockRequests"."BlockRequestId";


--
-- TOC entry 265 (class 1259 OID 33095)
-- Name: Business; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Business" (
    "BusinessId" integer NOT NULL,
    "Name" character varying(100) NOT NULL,
    "Address1" character varying(500),
    "Address2" character varying(500),
    "City" character varying(50),
    "RegionId" integer,
    "ZipCode" character varying(10),
    "PhoneNumber" character varying(20),
    "FaxNumber" character varying(20),
    "IsRegistered" bit(1),
    "CreatedBy" integer,
    "CreatedDate" timestamp without time zone NOT NULL,
    "ModifiedBy" integer,
    "ModifiedDate" timestamp without time zone,
    "Status" smallint,
    "IsDeleted" bit(1),
    "IP" character varying(20)
);


ALTER TABLE public."Business" OWNER TO postgres;

--
-- TOC entry 264 (class 1259 OID 33094)
-- Name: Business_BusinessId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Business_BusinessId_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."Business_BusinessId_seq" OWNER TO postgres;

--
-- TOC entry 5185 (class 0 OID 0)
-- Dependencies: 264
-- Name: Business_BusinessId_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Business_BusinessId_seq" OWNED BY public."Business"."BusinessId";


--
-- TOC entry 225 (class 1259 OID 32839)
-- Name: CaseTag; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."CaseTag" (
    "CaseTagId" integer NOT NULL,
    "Name" character varying(50) NOT NULL
);


ALTER TABLE public."CaseTag" OWNER TO postgres;

--
-- TOC entry 224 (class 1259 OID 32838)
-- Name: CaseTag_CaseTagId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."CaseTag_CaseTagId_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."CaseTag_CaseTagId_seq" OWNER TO postgres;

--
-- TOC entry 5186 (class 0 OID 0)
-- Dependencies: 224
-- Name: CaseTag_CaseTagId_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."CaseTag_CaseTagId_seq" OWNED BY public."CaseTag"."CaseTagId";


--
-- TOC entry 267 (class 1259 OID 33119)
-- Name: Concierge; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Concierge" (
    "ConciergeId" integer NOT NULL,
    "ConciergeName" character varying(100) NOT NULL,
    "Address" character varying(150),
    "Street" character varying(50) NOT NULL,
    "City" character varying(50) NOT NULL,
    "State" character varying(50) NOT NULL,
    "ZipCode" character varying(50) NOT NULL,
    "CreatedDate" timestamp without time zone NOT NULL,
    "RegionId" integer,
    "IP" character varying(20)
);


ALTER TABLE public."Concierge" OWNER TO postgres;

--
-- TOC entry 266 (class 1259 OID 33118)
-- Name: Concierge_ConciergeId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Concierge_ConciergeId_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."Concierge_ConciergeId_seq" OWNER TO postgres;

--
-- TOC entry 5187 (class 0 OID 0)
-- Dependencies: 266
-- Name: Concierge_ConciergeId_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Concierge_ConciergeId_seq" OWNED BY public."Concierge"."ConciergeId";


--
-- TOC entry 227 (class 1259 OID 32846)
-- Name: EmailLog; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."EmailLog" (
    "EmailLogID" integer NOT NULL,
    "EmailTemplate" text,
    "SubjectName" character varying(200) NOT NULL,
    "EmailID" character varying(200) NOT NULL,
    "ConfirmationNumber" character varying(200),
    "FilePath" text,
    "RoleId" integer,
    "RequestId" integer,
    "AdminId" integer,
    "PhysicianId" integer,
    "CreateDate" timestamp without time zone NOT NULL,
    "SentDate" timestamp without time zone,
    "IsEmailSent" bit(1),
    "SentTries" integer,
    "Action" integer
);


ALTER TABLE public."EmailLog" OWNER TO postgres;

--
-- TOC entry 226 (class 1259 OID 32845)
-- Name: EmailLog_EmailLogID_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."EmailLog_EmailLogID_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."EmailLog_EmailLogID_seq" OWNER TO postgres;

--
-- TOC entry 5188 (class 0 OID 0)
-- Dependencies: 226
-- Name: EmailLog_EmailLogID_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."EmailLog_EmailLogID_seq" OWNED BY public."EmailLog"."EmailLogID";


--
-- TOC entry 286 (class 1259 OID 49168)
-- Name: EncounterForm; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."EncounterForm" (
    "Id" integer NOT NULL,
    "RequestId" integer NOT NULL,
    "isFinalized" bit(1) DEFAULT '0'::"bit",
    history_illness text,
    medical_history text,
    "Date" timestamp without time zone,
    "Medications" text,
    "Allergies" text,
    "Temp" numeric,
    "HR" numeric,
    "RR" numeric,
    "BP(S)" integer,
    "BP(D)" integer,
    "O2" numeric,
    "Pain" text,
    "HEENT" text,
    "CV" text,
    "Chest" text,
    "ABD" text,
    "Extr" text,
    "Skin" text,
    "Neuro" text,
    "Other" text,
    "Diagnosis" text,
    "Treatment_Plan" text,
    medication_dispensed text,
    procedures text,
    "Follow_up" text
);


ALTER TABLE public."EncounterForm" OWNER TO postgres;

--
-- TOC entry 285 (class 1259 OID 49167)
-- Name: EncounterForm_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."EncounterForm_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."EncounterForm_Id_seq" OWNER TO postgres;

--
-- TOC entry 5189 (class 0 OID 0)
-- Dependencies: 285
-- Name: EncounterForm_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."EncounterForm_Id_seq" OWNED BY public."EncounterForm"."Id";


--
-- TOC entry 229 (class 1259 OID 32855)
-- Name: HealthProfessionalType; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."HealthProfessionalType" (
    "HealthProfessionalId" integer NOT NULL,
    "ProfessionName" character varying(50) NOT NULL,
    "CreatedDate" timestamp without time zone NOT NULL,
    "IsActive" bit(1),
    "IsDeleted" bit(1)
);


ALTER TABLE public."HealthProfessionalType" OWNER TO postgres;

--
-- TOC entry 228 (class 1259 OID 32854)
-- Name: HealthProfessionalType_HealthProfessionalId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."HealthProfessionalType_HealthProfessionalId_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."HealthProfessionalType_HealthProfessionalId_seq" OWNER TO postgres;

--
-- TOC entry 5190 (class 0 OID 0)
-- Dependencies: 228
-- Name: HealthProfessionalType_HealthProfessionalId_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."HealthProfessionalType_HealthProfessionalId_seq" OWNED BY public."HealthProfessionalType"."HealthProfessionalId";


--
-- TOC entry 231 (class 1259 OID 32862)
-- Name: HealthProfessionals; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."HealthProfessionals" (
    "VendorId" integer NOT NULL,
    "VendorName" character varying(100) NOT NULL,
    "Profession" integer,
    "FaxNumber" character varying(50) NOT NULL,
    "Address" character varying(150),
    "City" character varying(100),
    "State" character varying(50),
    "Zip" character varying(50),
    "RegionId" integer,
    "CreatedDate" timestamp without time zone NOT NULL,
    "ModifiedDate" timestamp without time zone,
    "PhoneNumber" character varying(100),
    "IsDeleted" bit(1),
    "IP" character varying(20),
    "Email" character varying(50),
    "BusinessContact" character varying(100)
);


ALTER TABLE public."HealthProfessionals" OWNER TO postgres;

--
-- TOC entry 230 (class 1259 OID 32861)
-- Name: HealthProfessionals_VendorId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."HealthProfessionals_VendorId_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."HealthProfessionals_VendorId_seq" OWNER TO postgres;

--
-- TOC entry 5191 (class 0 OID 0)
-- Dependencies: 230
-- Name: HealthProfessionals_VendorId_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."HealthProfessionals_VendorId_seq" OWNED BY public."HealthProfessionals"."VendorId";


--
-- TOC entry 233 (class 1259 OID 32876)
-- Name: Menu; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Menu" (
    "MenuId" integer NOT NULL,
    "Name" character varying(50) NOT NULL,
    "AccountType" smallint NOT NULL,
    "SortOrder" integer,
    CONSTRAINT "Menu_AccountType_check" CHECK (("AccountType" = ANY (ARRAY[1, 2])))
);


ALTER TABLE public."Menu" OWNER TO postgres;

--
-- TOC entry 232 (class 1259 OID 32875)
-- Name: Menu_MenuId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Menu_MenuId_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."Menu_MenuId_seq" OWNER TO postgres;

--
-- TOC entry 5192 (class 0 OID 0)
-- Dependencies: 232
-- Name: Menu_MenuId_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Menu_MenuId_seq" OWNED BY public."Menu"."MenuId";


--
-- TOC entry 235 (class 1259 OID 32884)
-- Name: OrderDetails; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."OrderDetails" (
    "Id" integer NOT NULL,
    "VendorId" integer,
    "RequestId" integer,
    "FaxNumber" character varying(50),
    "Email" character varying(50),
    "BusinessContact" character varying(100),
    "Prescription" text,
    "NoOfRefill" integer,
    "CreatedDate" timestamp without time zone,
    "CreatedBy" character varying(100)
);


ALTER TABLE public."OrderDetails" OWNER TO postgres;

--
-- TOC entry 234 (class 1259 OID 32883)
-- Name: OrderDetails_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."OrderDetails_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."OrderDetails_Id_seq" OWNER TO postgres;

--
-- TOC entry 5193 (class 0 OID 0)
-- Dependencies: 234
-- Name: OrderDetails_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."OrderDetails_Id_seq" OWNED BY public."OrderDetails"."Id";


--
-- TOC entry 237 (class 1259 OID 32893)
-- Name: Physician; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Physician" (
    "PhysicianId" integer NOT NULL,
    "AspNetUserId" integer,
    "FirstName" character varying(100) NOT NULL,
    "LastName" character varying(100),
    "Email" character varying(50) NOT NULL,
    "Mobile" character varying(20),
    "MedicalLicense" character varying(500),
    "Photo" character varying(100),
    "AdminNotes" character varying(500),
    "IsAgreementDoc" bit(1),
    "IsBackgroundDoc" bit(1),
    "IsTrainingDoc" bit(1),
    "IsNonDisclosureDoc" bit(1),
    "Address1" character varying(500),
    "Address2" character varying(500),
    "City" character varying(100),
    "RegionId" integer,
    "Zip" character varying(10),
    "AltPhone" character varying(20),
    "CreatedBy" integer,
    "CreatedDate" timestamp without time zone NOT NULL,
    "ModifiedBy" integer,
    "ModifiedDate" timestamp without time zone,
    "Status" smallint,
    "BusinessName" character varying(100) NOT NULL,
    "BusinessWebsite" character varying(200) NOT NULL,
    "IsDeleted" bit(1),
    "RoleId" integer,
    "NPINumber" character varying(500),
    "IsLicenseDoc" bit(1),
    "Signature" character varying(100),
    "IsCredentialDoc" bit(1),
    "IsTokenGenerate" bit(1),
    "SyncEmailAddress" character varying(50)
);


ALTER TABLE public."Physician" OWNER TO postgres;

--
-- TOC entry 239 (class 1259 OID 32917)
-- Name: PhysicianLocation; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."PhysicianLocation" (
    "LocationId" integer NOT NULL,
    "PhysicianId" integer NOT NULL,
    "Latitude" numeric(9,6),
    "Longitude" numeric(9,6),
    "CreatedDate" timestamp without time zone,
    "PhysicianName" character varying(50),
    "Address" character varying(500)
);


ALTER TABLE public."PhysicianLocation" OWNER TO postgres;

--
-- TOC entry 238 (class 1259 OID 32916)
-- Name: PhysicianLocation_LocationId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."PhysicianLocation_LocationId_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."PhysicianLocation_LocationId_seq" OWNER TO postgres;

--
-- TOC entry 5194 (class 0 OID 0)
-- Dependencies: 238
-- Name: PhysicianLocation_LocationId_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."PhysicianLocation_LocationId_seq" OWNED BY public."PhysicianLocation"."LocationId";


--
-- TOC entry 241 (class 1259 OID 32926)
-- Name: PhysicianNotification; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."PhysicianNotification" (
    id integer NOT NULL,
    "PhysicianId" integer NOT NULL,
    "IsNotificationStopped" bit(1) NOT NULL
);


ALTER TABLE public."PhysicianNotification" OWNER TO postgres;

--
-- TOC entry 240 (class 1259 OID 32925)
-- Name: PhysicianNotification_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."PhysicianNotification_id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."PhysicianNotification_id_seq" OWNER TO postgres;

--
-- TOC entry 5195 (class 0 OID 0)
-- Dependencies: 240
-- Name: PhysicianNotification_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."PhysicianNotification_id_seq" OWNED BY public."PhysicianNotification".id;


--
-- TOC entry 245 (class 1259 OID 32945)
-- Name: PhysicianRegion; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."PhysicianRegion" (
    "PhysicianRegionId" integer NOT NULL,
    "PhysicianId" integer NOT NULL,
    "RegionId" integer NOT NULL
);


ALTER TABLE public."PhysicianRegion" OWNER TO postgres;

--
-- TOC entry 244 (class 1259 OID 32944)
-- Name: PhysicianRegion_PhysicianRegionId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."PhysicianRegion_PhysicianRegionId_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."PhysicianRegion_PhysicianRegionId_seq" OWNER TO postgres;

--
-- TOC entry 5196 (class 0 OID 0)
-- Dependencies: 244
-- Name: PhysicianRegion_PhysicianRegionId_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."PhysicianRegion_PhysicianRegionId_seq" OWNED BY public."PhysicianRegion"."PhysicianRegionId";


--
-- TOC entry 236 (class 1259 OID 32892)
-- Name: Physician_PhysicianId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Physician_PhysicianId_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."Physician_PhysicianId_seq" OWNER TO postgres;

--
-- TOC entry 5197 (class 0 OID 0)
-- Dependencies: 236
-- Name: Physician_PhysicianId_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Physician_PhysicianId_seq" OWNED BY public."Physician"."PhysicianId";


--
-- TOC entry 243 (class 1259 OID 32938)
-- Name: Region; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Region" (
    "RegionId" integer NOT NULL,
    "Name" character varying(50) NOT NULL,
    "Abbreviation" character varying(50)
);


ALTER TABLE public."Region" OWNER TO postgres;

--
-- TOC entry 242 (class 1259 OID 32937)
-- Name: Region_RegionId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Region_RegionId_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."Region_RegionId_seq" OWNER TO postgres;

--
-- TOC entry 5198 (class 0 OID 0)
-- Dependencies: 242
-- Name: Region_RegionId_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Region_RegionId_seq" OWNED BY public."Region"."RegionId";


--
-- TOC entry 269 (class 1259 OID 33131)
-- Name: Request; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Request" (
    "RequestId" integer NOT NULL,
    "RequestTypeId" integer NOT NULL,
    "UserId" integer,
    "FirstName" character varying(100),
    "LastName" character varying(100),
    "PhoneNumber" character varying(23),
    "Email" character varying(50),
    "Status" smallint NOT NULL,
    "PhysicianId" integer,
    "ConfirmationNumber" character varying(20),
    "CreatedDate" timestamp without time zone NOT NULL,
    "IsDeleted" bit(1),
    "ModifiedDate" timestamp without time zone,
    "DeclinedBy" character varying(250),
    "IsUrgentEmailSent" bit(1),
    "LastWellnessDate" timestamp without time zone,
    "IsMobile" bit(1),
    "CallType" smallint,
    "CompletedByPhysician" bit(1),
    "LastReservationDate" timestamp without time zone,
    "AcceptedDate" timestamp without time zone,
    "RelationName" character varying(100),
    "CaseNumber" character varying(50),
    "IP" character varying(20),
    "CaseTag" character varying(50),
    "CaseTagPhysician" character varying(50),
    "PatientAccountId" character varying(128),
    "CreatedUserId" integer,
    "RequestClientId" integer NOT NULL,
    CONSTRAINT "Request_RequestTypeId_check" CHECK (("RequestTypeId" = ANY (ARRAY[1, 2, 3, 4]))),
    CONSTRAINT "Request_Status_check" CHECK (("Status" = ANY (ARRAY[1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15])))
);


ALTER TABLE public."Request" OWNER TO postgres;

--
-- TOC entry 271 (class 1259 OID 33152)
-- Name: RequestBusiness; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."RequestBusiness" (
    "RequestBusinessId" integer NOT NULL,
    "RequestId" integer NOT NULL,
    "BusinessId" integer NOT NULL,
    "IP" character varying(20)
);


ALTER TABLE public."RequestBusiness" OWNER TO postgres;

--
-- TOC entry 270 (class 1259 OID 33151)
-- Name: RequestBusiness_RequestBusinessId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."RequestBusiness_RequestBusinessId_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."RequestBusiness_RequestBusinessId_seq" OWNER TO postgres;

--
-- TOC entry 5199 (class 0 OID 0)
-- Dependencies: 270
-- Name: RequestBusiness_RequestBusinessId_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."RequestBusiness_RequestBusinessId_seq" OWNED BY public."RequestBusiness"."RequestBusinessId";


--
-- TOC entry 273 (class 1259 OID 33169)
-- Name: RequestClient; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."RequestClient" (
    "RequestClientId" integer NOT NULL,
    "FirstName" character varying(100) NOT NULL,
    "LastName" character varying(100),
    "PhoneNumber" character varying(23),
    "Location" character varying(100),
    "Address" character varying(500),
    "RegionId" integer,
    "NotiMobile" character varying(20),
    "NotiEmail" character varying(50),
    "Notes" character varying(500),
    "Email" character varying(50),
    "strMonth" character varying(20),
    "intYear" integer,
    "intDate" integer,
    "IsMobile" bit(1),
    "Street" character varying(100),
    "City" character varying(100),
    "State" character varying(100),
    "ZipCode" character varying(10),
    "CommunicationType" smallint,
    "RemindReservationCount" smallint,
    "RemindHouseCallCount" smallint,
    "IsSetFollowupSent" smallint,
    "IP" character varying(20),
    "IsReservationReminderSent" smallint,
    "Latitude" numeric(9,6),
    "Longitude" numeric(9,6)
);


ALTER TABLE public."RequestClient" OWNER TO postgres;

--
-- TOC entry 272 (class 1259 OID 33168)
-- Name: RequestClient_RequestClientId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."RequestClient_RequestClientId_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."RequestClient_RequestClientId_seq" OWNER TO postgres;

--
-- TOC entry 5200 (class 0 OID 0)
-- Dependencies: 272
-- Name: RequestClient_RequestClientId_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."RequestClient_RequestClientId_seq" OWNED BY public."RequestClient"."RequestClientId";


--
-- TOC entry 277 (class 1259 OID 33217)
-- Name: RequestClosed; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."RequestClosed" (
    "RequestClosedId" integer NOT NULL,
    "RequestId" integer NOT NULL,
    "RequestStatusLogId" integer NOT NULL,
    "PhyNotes" character varying(500),
    "ClientNotes" character varying(500),
    "IP" character varying(20)
);


ALTER TABLE public."RequestClosed" OWNER TO postgres;

--
-- TOC entry 276 (class 1259 OID 33216)
-- Name: RequestClosed_RequestClosedId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."RequestClosed_RequestClosedId_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."RequestClosed_RequestClosedId_seq" OWNER TO postgres;

--
-- TOC entry 5201 (class 0 OID 0)
-- Dependencies: 276
-- Name: RequestClosed_RequestClosedId_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."RequestClosed_RequestClosedId_seq" OWNED BY public."RequestClosed"."RequestClosedId";


--
-- TOC entry 279 (class 1259 OID 33236)
-- Name: RequestConcierge; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."RequestConcierge" (
    "Id" integer NOT NULL,
    "RequestId" integer NOT NULL,
    "ConciergeId" integer NOT NULL,
    "IP" character varying(20)
);


ALTER TABLE public."RequestConcierge" OWNER TO postgres;

--
-- TOC entry 278 (class 1259 OID 33235)
-- Name: RequestConcierge_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."RequestConcierge_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."RequestConcierge_Id_seq" OWNER TO postgres;

--
-- TOC entry 5202 (class 0 OID 0)
-- Dependencies: 278
-- Name: RequestConcierge_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."RequestConcierge_Id_seq" OWNED BY public."RequestConcierge"."Id";


--
-- TOC entry 281 (class 1259 OID 33253)
-- Name: RequestNotes; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."RequestNotes" (
    "RequestNotesId" integer NOT NULL,
    "RequestId" integer NOT NULL,
    "strMonth" character varying(20),
    "intYear" integer,
    "intDate" integer,
    "PhysicianNotes" character varying(500),
    "AdminNotes" character varying(500),
    "CreatedBy" integer NOT NULL,
    "CreatedDate" timestamp without time zone NOT NULL,
    "ModifiedBy" integer,
    "ModifiedDate" timestamp without time zone,
    "IP" character varying(20),
    "AdministrativeNotes" character varying(500)
);


ALTER TABLE public."RequestNotes" OWNER TO postgres;

--
-- TOC entry 280 (class 1259 OID 33252)
-- Name: RequestNotes_RequestNotesId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."RequestNotes_RequestNotesId_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."RequestNotes_RequestNotesId_seq" OWNER TO postgres;

--
-- TOC entry 5203 (class 0 OID 0)
-- Dependencies: 280
-- Name: RequestNotes_RequestNotesId_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."RequestNotes_RequestNotesId_seq" OWNED BY public."RequestNotes"."RequestNotesId";


--
-- TOC entry 275 (class 1259 OID 33188)
-- Name: RequestStatusLog; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."RequestStatusLog" (
    "RequestStatusLogId" integer NOT NULL,
    "RequestId" integer NOT NULL,
    "Status" smallint NOT NULL,
    "PhysicianId" integer,
    "AdminId" integer,
    "TransToPhysicianId" integer,
    "Notes" character varying(500),
    "CreatedDate" timestamp without time zone NOT NULL,
    "IP" character varying(20),
    "TransToAdmin" bit(1)
);


ALTER TABLE public."RequestStatusLog" OWNER TO postgres;

--
-- TOC entry 274 (class 1259 OID 33187)
-- Name: RequestStatusLog_RequestStatusLogId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."RequestStatusLog_RequestStatusLogId_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."RequestStatusLog_RequestStatusLogId_seq" OWNER TO postgres;

--
-- TOC entry 5204 (class 0 OID 0)
-- Dependencies: 274
-- Name: RequestStatusLog_RequestStatusLogId_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."RequestStatusLog_RequestStatusLogId_seq" OWNED BY public."RequestStatusLog"."RequestStatusLogId";


--
-- TOC entry 247 (class 1259 OID 32962)
-- Name: RequestType; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."RequestType" (
    "RequestTypeId" integer NOT NULL,
    "Name" character varying(50) NOT NULL
);


ALTER TABLE public."RequestType" OWNER TO postgres;

--
-- TOC entry 246 (class 1259 OID 32961)
-- Name: RequestType_RequestTypeId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."RequestType_RequestTypeId_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."RequestType_RequestTypeId_seq" OWNER TO postgres;

--
-- TOC entry 5205 (class 0 OID 0)
-- Dependencies: 246
-- Name: RequestType_RequestTypeId_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."RequestType_RequestTypeId_seq" OWNED BY public."RequestType"."RequestTypeId";


--
-- TOC entry 283 (class 1259 OID 33277)
-- Name: RequestWiseFile; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."RequestWiseFile" (
    "RequestWiseFileID" integer NOT NULL,
    "RequestId" integer NOT NULL,
    "FileName" character varying(500) NOT NULL,
    "CreatedDate" timestamp without time zone NOT NULL,
    "PhysicianId" integer,
    "AdminId" integer,
    "DocType" smallint,
    "IsFrontSide" bit(1),
    "IsCompensation" bit(1),
    "IP" character varying(20),
    "IsFinalize" bit(1),
    "IsDeleted" bit(1),
    "IsPatientRecords" bit(1)
);


ALTER TABLE public."RequestWiseFile" OWNER TO postgres;

--
-- TOC entry 282 (class 1259 OID 33276)
-- Name: RequestWiseFile_RequestWiseFileID_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."RequestWiseFile_RequestWiseFileID_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."RequestWiseFile_RequestWiseFileID_seq" OWNER TO postgres;

--
-- TOC entry 5206 (class 0 OID 0)
-- Dependencies: 282
-- Name: RequestWiseFile_RequestWiseFileID_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."RequestWiseFile_RequestWiseFileID_seq" OWNED BY public."RequestWiseFile"."RequestWiseFileID";


--
-- TOC entry 268 (class 1259 OID 33130)
-- Name: Request_RequestId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Request_RequestId_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."Request_RequestId_seq" OWNER TO postgres;

--
-- TOC entry 5207 (class 0 OID 0)
-- Dependencies: 268
-- Name: Request_RequestId_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Request_RequestId_seq" OWNED BY public."Request"."RequestId";


--
-- TOC entry 249 (class 1259 OID 32969)
-- Name: Role; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Role" (
    "RoleId" integer NOT NULL,
    "Name" character varying(50) NOT NULL,
    "AccountType" smallint NOT NULL,
    "CreatedBy" character varying(128) NOT NULL,
    "CreatedDate" timestamp without time zone NOT NULL,
    "ModifiedBy" character varying(128),
    "ModifiedDate" timestamp without time zone,
    "IsDeleted" bit(1) NOT NULL,
    "IP" character varying(20),
    CONSTRAINT "Role_AccountType_check" CHECK (("AccountType" = ANY (ARRAY[1, 2])))
);


ALTER TABLE public."Role" OWNER TO postgres;

--
-- TOC entry 251 (class 1259 OID 32977)
-- Name: RoleMenu; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."RoleMenu" (
    "RoleMenuId" integer NOT NULL,
    "RoleId" integer NOT NULL,
    "MenuId" integer NOT NULL
);


ALTER TABLE public."RoleMenu" OWNER TO postgres;

--
-- TOC entry 250 (class 1259 OID 32976)
-- Name: RoleMenu_RoleMenuId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."RoleMenu_RoleMenuId_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."RoleMenu_RoleMenuId_seq" OWNER TO postgres;

--
-- TOC entry 5208 (class 0 OID 0)
-- Dependencies: 250
-- Name: RoleMenu_RoleMenuId_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."RoleMenu_RoleMenuId_seq" OWNED BY public."RoleMenu"."RoleMenuId";


--
-- TOC entry 248 (class 1259 OID 32968)
-- Name: Role_RoleId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Role_RoleId_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."Role_RoleId_seq" OWNER TO postgres;

--
-- TOC entry 5209 (class 0 OID 0)
-- Dependencies: 248
-- Name: Role_RoleId_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Role_RoleId_seq" OWNED BY public."Role"."RoleId";


--
-- TOC entry 259 (class 1259 OID 33045)
-- Name: SMSLog; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."SMSLog" (
    "SMSLogID" integer NOT NULL,
    "SMSTemplate" text NOT NULL,
    "MobileNumber" character varying(50) NOT NULL,
    "ConfirmationNumber" text,
    "RoleId" integer,
    "AdminId" integer,
    "RequestId" integer,
    "PhysicianId" integer,
    "CreateDate" timestamp without time zone NOT NULL,
    "SentDate" timestamp without time zone,
    "IsSMSSent" bit(1),
    "SentTries" integer NOT NULL,
    "Action" integer
);


ALTER TABLE public."SMSLog" OWNER TO postgres;

--
-- TOC entry 258 (class 1259 OID 33044)
-- Name: SMSLog_SMSLogID_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."SMSLog_SMSLogID_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."SMSLog_SMSLogID_seq" OWNER TO postgres;

--
-- TOC entry 5210 (class 0 OID 0)
-- Dependencies: 258
-- Name: SMSLog_SMSLogID_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."SMSLog_SMSLogID_seq" OWNED BY public."SMSLog"."SMSLogID";


--
-- TOC entry 253 (class 1259 OID 32994)
-- Name: Shift; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Shift" (
    "ShiftId" integer NOT NULL,
    "PhysicianId" integer NOT NULL,
    "StartDate" date NOT NULL,
    "IsRepeat" bit(1) NOT NULL,
    "WeekDays" character(7),
    "RepeatUpto" integer,
    "CreatedBy" integer NOT NULL,
    "CreatedDate" timestamp without time zone NOT NULL,
    "IP" character varying(20)
);


ALTER TABLE public."Shift" OWNER TO postgres;

--
-- TOC entry 255 (class 1259 OID 33011)
-- Name: ShiftDetail; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."ShiftDetail" (
    "ShiftDetailId" integer NOT NULL,
    "ShiftId" integer NOT NULL,
    "ShiftDate" timestamp without time zone NOT NULL,
    "RegionId" integer,
    "StartTime" time without time zone NOT NULL,
    "EndTime" time without time zone NOT NULL,
    "Status" smallint NOT NULL,
    "IsDeleted" bit(1) NOT NULL,
    "ModifiedBy" integer,
    "ModifiedDate" timestamp without time zone,
    "LastRunningDate" timestamp without time zone,
    "EventId" character varying(100),
    "IsSync" bit(1)
);


ALTER TABLE public."ShiftDetail" OWNER TO postgres;

--
-- TOC entry 257 (class 1259 OID 33028)
-- Name: ShiftDetailRegion; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."ShiftDetailRegion" (
    "ShiftDetailRegionId" integer NOT NULL,
    "ShiftDetailId" integer NOT NULL,
    "RegionId" integer NOT NULL,
    "IsDeleted" bit(1)
);


ALTER TABLE public."ShiftDetailRegion" OWNER TO postgres;

--
-- TOC entry 256 (class 1259 OID 33027)
-- Name: ShiftDetailRegion_ShiftDetailRegionId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."ShiftDetailRegion_ShiftDetailRegionId_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."ShiftDetailRegion_ShiftDetailRegionId_seq" OWNER TO postgres;

--
-- TOC entry 5211 (class 0 OID 0)
-- Dependencies: 256
-- Name: ShiftDetailRegion_ShiftDetailRegionId_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."ShiftDetailRegion_ShiftDetailRegionId_seq" OWNED BY public."ShiftDetailRegion"."ShiftDetailRegionId";


--
-- TOC entry 254 (class 1259 OID 33010)
-- Name: ShiftDetail_ShiftDetailId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."ShiftDetail_ShiftDetailId_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."ShiftDetail_ShiftDetailId_seq" OWNER TO postgres;

--
-- TOC entry 5212 (class 0 OID 0)
-- Dependencies: 254
-- Name: ShiftDetail_ShiftDetailId_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."ShiftDetail_ShiftDetailId_seq" OWNED BY public."ShiftDetail"."ShiftDetailId";


--
-- TOC entry 252 (class 1259 OID 32993)
-- Name: Shift_ShiftId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Shift_ShiftId_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."Shift_ShiftId_seq" OWNER TO postgres;

--
-- TOC entry 5213 (class 0 OID 0)
-- Dependencies: 252
-- Name: Shift_ShiftId_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Shift_ShiftId_seq" OWNED BY public."Shift"."ShiftId";


--
-- TOC entry 261 (class 1259 OID 33054)
-- Name: User; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."User" (
    "UserId" integer NOT NULL,
    "AspNetUserId" integer,
    "FirstName" character varying(100) NOT NULL,
    "LastName" character varying(100),
    "Email" character varying(50) NOT NULL,
    "Mobile" character varying(20),
    "IsMobile" bit(1),
    "Street" character varying(100),
    "City" character varying(100),
    "State" character varying(100),
    "RegionId" integer,
    "ZipCode" character varying(10),
    "strMonth" character varying(20),
    "intYear" integer,
    "intDate" integer,
    "CreatedBy" integer NOT NULL,
    "CreatedDate" timestamp without time zone NOT NULL,
    "ModifiedBy" integer,
    "ModifiedDate" timestamp without time zone,
    "Status" smallint,
    "IsDeleted" bit(1),
    "IP" character varying(20),
    "IsRequestWithEmail" bit(1)
);


ALTER TABLE public."User" OWNER TO postgres;

--
-- TOC entry 260 (class 1259 OID 33053)
-- Name: User_UserId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."User_UserId_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."User_UserId_seq" OWNER TO postgres;

--
-- TOC entry 5214 (class 0 OID 0)
-- Dependencies: 260
-- Name: User_UserId_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."User_UserId_seq" OWNED BY public."User"."UserId";


--
-- TOC entry 284 (class 1259 OID 49152)
-- Name: passwordreset; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.passwordreset (
    token character varying(256) NOT NULL,
    email character varying(256),
    isupdated bit(1),
    createddate timestamp without time zone
);


ALTER TABLE public.passwordreset OWNER TO postgres;

--
-- TOC entry 4869 (class 2604 OID 32794)
-- Name: Admin AdminId; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Admin" ALTER COLUMN "AdminId" SET DEFAULT nextval('public."Admin_AdminId_seq"'::regclass);


--
-- TOC entry 4890 (class 2604 OID 33081)
-- Name: AdminRegion AdminRegionId; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AdminRegion" ALTER COLUMN "AdminRegionId" SET DEFAULT nextval('public."AdminRegion_AdminRegionId_seq"'::regclass);


--
-- TOC entry 4867 (class 2604 OID 32778)
-- Name: AspNetRoles Id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetRoles" ALTER COLUMN "Id" SET DEFAULT nextval('public."AspNetRoles_Id_seq"'::regclass);


--
-- TOC entry 4868 (class 2604 OID 32785)
-- Name: AspNetUsers Id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetUsers" ALTER COLUMN "Id" SET DEFAULT nextval('public."AspNetUsers_Id_seq"'::regclass);


--
-- TOC entry 4870 (class 2604 OID 32833)
-- Name: BlockRequests BlockRequestId; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."BlockRequests" ALTER COLUMN "BlockRequestId" SET DEFAULT nextval('public."BlockRequests_BlockRequestId_seq"'::regclass);


--
-- TOC entry 4891 (class 2604 OID 33098)
-- Name: Business BusinessId; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Business" ALTER COLUMN "BusinessId" SET DEFAULT nextval('public."Business_BusinessId_seq"'::regclass);


--
-- TOC entry 4871 (class 2604 OID 32842)
-- Name: CaseTag CaseTagId; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CaseTag" ALTER COLUMN "CaseTagId" SET DEFAULT nextval('public."CaseTag_CaseTagId_seq"'::regclass);


--
-- TOC entry 4892 (class 2604 OID 33122)
-- Name: Concierge ConciergeId; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Concierge" ALTER COLUMN "ConciergeId" SET DEFAULT nextval('public."Concierge_ConciergeId_seq"'::regclass);


--
-- TOC entry 4872 (class 2604 OID 32849)
-- Name: EmailLog EmailLogID; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."EmailLog" ALTER COLUMN "EmailLogID" SET DEFAULT nextval('public."EmailLog_EmailLogID_seq"'::regclass);


--
-- TOC entry 4901 (class 2604 OID 49171)
-- Name: EncounterForm Id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."EncounterForm" ALTER COLUMN "Id" SET DEFAULT nextval('public."EncounterForm_Id_seq"'::regclass);


--
-- TOC entry 4873 (class 2604 OID 32858)
-- Name: HealthProfessionalType HealthProfessionalId; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."HealthProfessionalType" ALTER COLUMN "HealthProfessionalId" SET DEFAULT nextval('public."HealthProfessionalType_HealthProfessionalId_seq"'::regclass);


--
-- TOC entry 4874 (class 2604 OID 32865)
-- Name: HealthProfessionals VendorId; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."HealthProfessionals" ALTER COLUMN "VendorId" SET DEFAULT nextval('public."HealthProfessionals_VendorId_seq"'::regclass);


--
-- TOC entry 4875 (class 2604 OID 32879)
-- Name: Menu MenuId; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Menu" ALTER COLUMN "MenuId" SET DEFAULT nextval('public."Menu_MenuId_seq"'::regclass);


--
-- TOC entry 4876 (class 2604 OID 32887)
-- Name: OrderDetails Id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."OrderDetails" ALTER COLUMN "Id" SET DEFAULT nextval('public."OrderDetails_Id_seq"'::regclass);


--
-- TOC entry 4877 (class 2604 OID 32896)
-- Name: Physician PhysicianId; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Physician" ALTER COLUMN "PhysicianId" SET DEFAULT nextval('public."Physician_PhysicianId_seq"'::regclass);


--
-- TOC entry 4878 (class 2604 OID 32920)
-- Name: PhysicianLocation LocationId; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."PhysicianLocation" ALTER COLUMN "LocationId" SET DEFAULT nextval('public."PhysicianLocation_LocationId_seq"'::regclass);


--
-- TOC entry 4879 (class 2604 OID 32929)
-- Name: PhysicianNotification id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."PhysicianNotification" ALTER COLUMN id SET DEFAULT nextval('public."PhysicianNotification_id_seq"'::regclass);


--
-- TOC entry 4881 (class 2604 OID 32948)
-- Name: PhysicianRegion PhysicianRegionId; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."PhysicianRegion" ALTER COLUMN "PhysicianRegionId" SET DEFAULT nextval('public."PhysicianRegion_PhysicianRegionId_seq"'::regclass);


--
-- TOC entry 4880 (class 2604 OID 32941)
-- Name: Region RegionId; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Region" ALTER COLUMN "RegionId" SET DEFAULT nextval('public."Region_RegionId_seq"'::regclass);


--
-- TOC entry 4893 (class 2604 OID 33134)
-- Name: Request RequestId; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Request" ALTER COLUMN "RequestId" SET DEFAULT nextval('public."Request_RequestId_seq"'::regclass);


--
-- TOC entry 4894 (class 2604 OID 33155)
-- Name: RequestBusiness RequestBusinessId; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RequestBusiness" ALTER COLUMN "RequestBusinessId" SET DEFAULT nextval('public."RequestBusiness_RequestBusinessId_seq"'::regclass);


--
-- TOC entry 4895 (class 2604 OID 33172)
-- Name: RequestClient RequestClientId; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RequestClient" ALTER COLUMN "RequestClientId" SET DEFAULT nextval('public."RequestClient_RequestClientId_seq"'::regclass);


--
-- TOC entry 4897 (class 2604 OID 33220)
-- Name: RequestClosed RequestClosedId; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RequestClosed" ALTER COLUMN "RequestClosedId" SET DEFAULT nextval('public."RequestClosed_RequestClosedId_seq"'::regclass);


--
-- TOC entry 4898 (class 2604 OID 33239)
-- Name: RequestConcierge Id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RequestConcierge" ALTER COLUMN "Id" SET DEFAULT nextval('public."RequestConcierge_Id_seq"'::regclass);


--
-- TOC entry 4899 (class 2604 OID 33256)
-- Name: RequestNotes RequestNotesId; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RequestNotes" ALTER COLUMN "RequestNotesId" SET DEFAULT nextval('public."RequestNotes_RequestNotesId_seq"'::regclass);


--
-- TOC entry 4896 (class 2604 OID 33191)
-- Name: RequestStatusLog RequestStatusLogId; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RequestStatusLog" ALTER COLUMN "RequestStatusLogId" SET DEFAULT nextval('public."RequestStatusLog_RequestStatusLogId_seq"'::regclass);


--
-- TOC entry 4882 (class 2604 OID 32965)
-- Name: RequestType RequestTypeId; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RequestType" ALTER COLUMN "RequestTypeId" SET DEFAULT nextval('public."RequestType_RequestTypeId_seq"'::regclass);


--
-- TOC entry 4900 (class 2604 OID 33280)
-- Name: RequestWiseFile RequestWiseFileID; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RequestWiseFile" ALTER COLUMN "RequestWiseFileID" SET DEFAULT nextval('public."RequestWiseFile_RequestWiseFileID_seq"'::regclass);


--
-- TOC entry 4883 (class 2604 OID 32972)
-- Name: Role RoleId; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Role" ALTER COLUMN "RoleId" SET DEFAULT nextval('public."Role_RoleId_seq"'::regclass);


--
-- TOC entry 4884 (class 2604 OID 32980)
-- Name: RoleMenu RoleMenuId; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RoleMenu" ALTER COLUMN "RoleMenuId" SET DEFAULT nextval('public."RoleMenu_RoleMenuId_seq"'::regclass);


--
-- TOC entry 4888 (class 2604 OID 33048)
-- Name: SMSLog SMSLogID; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SMSLog" ALTER COLUMN "SMSLogID" SET DEFAULT nextval('public."SMSLog_SMSLogID_seq"'::regclass);


--
-- TOC entry 4885 (class 2604 OID 32997)
-- Name: Shift ShiftId; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Shift" ALTER COLUMN "ShiftId" SET DEFAULT nextval('public."Shift_ShiftId_seq"'::regclass);


--
-- TOC entry 4886 (class 2604 OID 33014)
-- Name: ShiftDetail ShiftDetailId; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ShiftDetail" ALTER COLUMN "ShiftDetailId" SET DEFAULT nextval('public."ShiftDetail_ShiftDetailId_seq"'::regclass);


--
-- TOC entry 4887 (class 2604 OID 33031)
-- Name: ShiftDetailRegion ShiftDetailRegionId; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ShiftDetailRegion" ALTER COLUMN "ShiftDetailRegionId" SET DEFAULT nextval('public."ShiftDetailRegion_ShiftDetailRegionId_seq"'::regclass);


--
-- TOC entry 4889 (class 2604 OID 33057)
-- Name: User UserId; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."User" ALTER COLUMN "UserId" SET DEFAULT nextval('public."User_UserId_seq"'::regclass);


--
-- TOC entry 4956 (class 2606 OID 33083)
-- Name: AdminRegion AdminRegion_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AdminRegion"
    ADD CONSTRAINT "AdminRegion_pkey" PRIMARY KEY ("AdminRegionId");


--
-- TOC entry 4912 (class 2606 OID 32798)
-- Name: Admin Admin_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Admin"
    ADD CONSTRAINT "Admin_pkey" PRIMARY KEY ("AdminId");


--
-- TOC entry 4908 (class 2606 OID 32780)
-- Name: AspNetRoles AspNetRoles_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetRoles"
    ADD CONSTRAINT "AspNetRoles_pkey" PRIMARY KEY ("Id");


--
-- TOC entry 4914 (class 2606 OID 49160)
-- Name: AspNetUserRoles AspNetUserRoles_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetUserRoles"
    ADD CONSTRAINT "AspNetUserRoles_pkey" PRIMARY KEY ("UserId");


--
-- TOC entry 4910 (class 2606 OID 32789)
-- Name: AspNetUsers AspNetUsers_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetUsers"
    ADD CONSTRAINT "AspNetUsers_pkey" PRIMARY KEY ("Id");


--
-- TOC entry 4916 (class 2606 OID 32837)
-- Name: BlockRequests BlockRequests_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."BlockRequests"
    ADD CONSTRAINT "BlockRequests_pkey" PRIMARY KEY ("BlockRequestId");


--
-- TOC entry 4958 (class 2606 OID 33102)
-- Name: Business Business_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Business"
    ADD CONSTRAINT "Business_pkey" PRIMARY KEY ("BusinessId");


--
-- TOC entry 4918 (class 2606 OID 32844)
-- Name: CaseTag CaseTag_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CaseTag"
    ADD CONSTRAINT "CaseTag_pkey" PRIMARY KEY ("CaseTagId");


--
-- TOC entry 4960 (class 2606 OID 33124)
-- Name: Concierge Concierge_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Concierge"
    ADD CONSTRAINT "Concierge_pkey" PRIMARY KEY ("ConciergeId");


--
-- TOC entry 4920 (class 2606 OID 32853)
-- Name: EmailLog EmailLog_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."EmailLog"
    ADD CONSTRAINT "EmailLog_pkey" PRIMARY KEY ("EmailLogID");


--
-- TOC entry 4980 (class 2606 OID 49176)
-- Name: EncounterForm EncounterForm_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."EncounterForm"
    ADD CONSTRAINT "EncounterForm_pkey" PRIMARY KEY ("Id");


--
-- TOC entry 4922 (class 2606 OID 32860)
-- Name: HealthProfessionalType HealthProfessionalType_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."HealthProfessionalType"
    ADD CONSTRAINT "HealthProfessionalType_pkey" PRIMARY KEY ("HealthProfessionalId");


--
-- TOC entry 4924 (class 2606 OID 32869)
-- Name: HealthProfessionals HealthProfessionals_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."HealthProfessionals"
    ADD CONSTRAINT "HealthProfessionals_pkey" PRIMARY KEY ("VendorId");


--
-- TOC entry 4926 (class 2606 OID 32882)
-- Name: Menu Menu_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Menu"
    ADD CONSTRAINT "Menu_pkey" PRIMARY KEY ("MenuId");


--
-- TOC entry 4928 (class 2606 OID 32891)
-- Name: OrderDetails OrderDetails_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."OrderDetails"
    ADD CONSTRAINT "OrderDetails_pkey" PRIMARY KEY ("Id");


--
-- TOC entry 4932 (class 2606 OID 32924)
-- Name: PhysicianLocation PhysicianLocation_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."PhysicianLocation"
    ADD CONSTRAINT "PhysicianLocation_pkey" PRIMARY KEY ("LocationId");


--
-- TOC entry 4934 (class 2606 OID 32931)
-- Name: PhysicianNotification PhysicianNotification_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."PhysicianNotification"
    ADD CONSTRAINT "PhysicianNotification_pkey" PRIMARY KEY (id);


--
-- TOC entry 4938 (class 2606 OID 32950)
-- Name: PhysicianRegion PhysicianRegion_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."PhysicianRegion"
    ADD CONSTRAINT "PhysicianRegion_pkey" PRIMARY KEY ("PhysicianRegionId");


--
-- TOC entry 4930 (class 2606 OID 32900)
-- Name: Physician Physician_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Physician"
    ADD CONSTRAINT "Physician_pkey" PRIMARY KEY ("PhysicianId");


--
-- TOC entry 4936 (class 2606 OID 32943)
-- Name: Region Region_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Region"
    ADD CONSTRAINT "Region_pkey" PRIMARY KEY ("RegionId");


--
-- TOC entry 4964 (class 2606 OID 33157)
-- Name: RequestBusiness RequestBusiness_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RequestBusiness"
    ADD CONSTRAINT "RequestBusiness_pkey" PRIMARY KEY ("RequestBusinessId");


--
-- TOC entry 4966 (class 2606 OID 33176)
-- Name: RequestClient RequestClient_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RequestClient"
    ADD CONSTRAINT "RequestClient_pkey" PRIMARY KEY ("RequestClientId");


--
-- TOC entry 4970 (class 2606 OID 33224)
-- Name: RequestClosed RequestClosed_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RequestClosed"
    ADD CONSTRAINT "RequestClosed_pkey" PRIMARY KEY ("RequestClosedId");


--
-- TOC entry 4972 (class 2606 OID 33241)
-- Name: RequestConcierge RequestConcierge_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RequestConcierge"
    ADD CONSTRAINT "RequestConcierge_pkey" PRIMARY KEY ("Id");


--
-- TOC entry 4974 (class 2606 OID 33260)
-- Name: RequestNotes RequestNotes_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RequestNotes"
    ADD CONSTRAINT "RequestNotes_pkey" PRIMARY KEY ("RequestNotesId");


--
-- TOC entry 4968 (class 2606 OID 33195)
-- Name: RequestStatusLog RequestStatusLog_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RequestStatusLog"
    ADD CONSTRAINT "RequestStatusLog_pkey" PRIMARY KEY ("RequestStatusLogId");


--
-- TOC entry 4940 (class 2606 OID 32967)
-- Name: RequestType RequestType_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RequestType"
    ADD CONSTRAINT "RequestType_pkey" PRIMARY KEY ("RequestTypeId");


--
-- TOC entry 4976 (class 2606 OID 33284)
-- Name: RequestWiseFile RequestWiseFile_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RequestWiseFile"
    ADD CONSTRAINT "RequestWiseFile_pkey" PRIMARY KEY ("RequestWiseFileID");


--
-- TOC entry 4962 (class 2606 OID 33140)
-- Name: Request Request_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Request"
    ADD CONSTRAINT "Request_pkey" PRIMARY KEY ("RequestId");


--
-- TOC entry 4944 (class 2606 OID 32982)
-- Name: RoleMenu RoleMenu_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RoleMenu"
    ADD CONSTRAINT "RoleMenu_pkey" PRIMARY KEY ("RoleMenuId");


--
-- TOC entry 4942 (class 2606 OID 32975)
-- Name: Role Role_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Role"
    ADD CONSTRAINT "Role_pkey" PRIMARY KEY ("RoleId");


--
-- TOC entry 4952 (class 2606 OID 33052)
-- Name: SMSLog SMSLog_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SMSLog"
    ADD CONSTRAINT "SMSLog_pkey" PRIMARY KEY ("SMSLogID");


--
-- TOC entry 4950 (class 2606 OID 33033)
-- Name: ShiftDetailRegion ShiftDetailRegion_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ShiftDetailRegion"
    ADD CONSTRAINT "ShiftDetailRegion_pkey" PRIMARY KEY ("ShiftDetailRegionId");


--
-- TOC entry 4948 (class 2606 OID 33016)
-- Name: ShiftDetail ShiftDetail_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ShiftDetail"
    ADD CONSTRAINT "ShiftDetail_pkey" PRIMARY KEY ("ShiftDetailId");


--
-- TOC entry 4946 (class 2606 OID 32999)
-- Name: Shift Shift_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Shift"
    ADD CONSTRAINT "Shift_pkey" PRIMARY KEY ("ShiftId");


--
-- TOC entry 4954 (class 2606 OID 33061)
-- Name: User User_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."User"
    ADD CONSTRAINT "User_pkey" PRIMARY KEY ("UserId");


--
-- TOC entry 4978 (class 2606 OID 49158)
-- Name: passwordreset passwordreset_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.passwordreset
    ADD CONSTRAINT passwordreset_pkey PRIMARY KEY (token);


--
-- TOC entry 4981 (class 2606 OID 32799)
-- Name: Admin Admin_AspNetUserId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Admin"
    ADD CONSTRAINT "Admin_AspNetUserId_fkey" FOREIGN KEY ("AspNetUserId") REFERENCES public."AspNetUsers"("Id");


--
-- TOC entry 4982 (class 2606 OID 32804)
-- Name: Admin Admin_CreatedBy_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Admin"
    ADD CONSTRAINT "Admin_CreatedBy_fkey" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- TOC entry 4983 (class 2606 OID 32809)
-- Name: Admin Admin_ModifiedBy_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Admin"
    ADD CONSTRAINT "Admin_ModifiedBy_fkey" FOREIGN KEY ("ModifiedBy") REFERENCES public."AspNetUsers"("Id");


--
-- TOC entry 4984 (class 2606 OID 32824)
-- Name: AspNetUserRoles AspNetUserRoles_RoleId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetUserRoles"
    ADD CONSTRAINT "AspNetUserRoles_RoleId_fkey" FOREIGN KEY ("RoleId") REFERENCES public."AspNetRoles"("Id");


--
-- TOC entry 4985 (class 2606 OID 32819)
-- Name: AspNetUserRoles AspNetUserRoles_UserId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetUserRoles"
    ADD CONSTRAINT "AspNetUserRoles_UserId_fkey" FOREIGN KEY ("UserId") REFERENCES public."AspNetUsers"("Id");


--
-- TOC entry 5006 (class 2606 OID 33108)
-- Name: Business Business_CreatedBy_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Business"
    ADD CONSTRAINT "Business_CreatedBy_fkey" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- TOC entry 5007 (class 2606 OID 33113)
-- Name: Business Business_ModifiedBy_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Business"
    ADD CONSTRAINT "Business_ModifiedBy_fkey" FOREIGN KEY ("ModifiedBy") REFERENCES public."AspNetUsers"("Id");


--
-- TOC entry 5008 (class 2606 OID 33103)
-- Name: Business Business_RegionId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Business"
    ADD CONSTRAINT "Business_RegionId_fkey" FOREIGN KEY ("RegionId") REFERENCES public."Region"("RegionId");


--
-- TOC entry 5009 (class 2606 OID 33125)
-- Name: Concierge Concierge_RegionId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Concierge"
    ADD CONSTRAINT "Concierge_RegionId_fkey" FOREIGN KEY ("RegionId") REFERENCES public."Region"("RegionId");


--
-- TOC entry 5004 (class 2606 OID 33084)
-- Name: AdminRegion FK_AdminRegion_AdminId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AdminRegion"
    ADD CONSTRAINT "FK_AdminRegion_AdminId" FOREIGN KEY ("AdminId") REFERENCES public."Admin"("AdminId");


--
-- TOC entry 5005 (class 2606 OID 33089)
-- Name: AdminRegion FK_AdminRegion_RegionId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AdminRegion"
    ADD CONSTRAINT "FK_AdminRegion_RegionId" FOREIGN KEY ("RegionId") REFERENCES public."Region"("RegionId");


--
-- TOC entry 4986 (class 2606 OID 32870)
-- Name: HealthProfessionals HealthProfessionals_Profession_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."HealthProfessionals"
    ADD CONSTRAINT "HealthProfessionals_Profession_fkey" FOREIGN KEY ("Profession") REFERENCES public."HealthProfessionalType"("HealthProfessionalId");


--
-- TOC entry 4990 (class 2606 OID 32932)
-- Name: PhysicianNotification PhysicianNotification_PhysicianId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."PhysicianNotification"
    ADD CONSTRAINT "PhysicianNotification_PhysicianId_fkey" FOREIGN KEY ("PhysicianId") REFERENCES public."Physician"("PhysicianId");


--
-- TOC entry 4991 (class 2606 OID 32951)
-- Name: PhysicianRegion PhysicianRegion_PhysicianId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."PhysicianRegion"
    ADD CONSTRAINT "PhysicianRegion_PhysicianId_fkey" FOREIGN KEY ("PhysicianId") REFERENCES public."Physician"("PhysicianId");


--
-- TOC entry 4992 (class 2606 OID 32956)
-- Name: PhysicianRegion PhysicianRegion_RegionId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."PhysicianRegion"
    ADD CONSTRAINT "PhysicianRegion_RegionId_fkey" FOREIGN KEY ("RegionId") REFERENCES public."Region"("RegionId");


--
-- TOC entry 4987 (class 2606 OID 32901)
-- Name: Physician Physician_AspNetUserId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Physician"
    ADD CONSTRAINT "Physician_AspNetUserId_fkey" FOREIGN KEY ("AspNetUserId") REFERENCES public."AspNetUsers"("Id");


--
-- TOC entry 4988 (class 2606 OID 32906)
-- Name: Physician Physician_CreatedBy_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Physician"
    ADD CONSTRAINT "Physician_CreatedBy_fkey" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- TOC entry 4989 (class 2606 OID 32911)
-- Name: Physician Physician_ModifiedBy_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Physician"
    ADD CONSTRAINT "Physician_ModifiedBy_fkey" FOREIGN KEY ("ModifiedBy") REFERENCES public."AspNetUsers"("Id");


--
-- TOC entry 5013 (class 2606 OID 33163)
-- Name: RequestBusiness RequestBusiness_BusinessId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RequestBusiness"
    ADD CONSTRAINT "RequestBusiness_BusinessId_fkey" FOREIGN KEY ("BusinessId") REFERENCES public."Business"("BusinessId");


--
-- TOC entry 5014 (class 2606 OID 33158)
-- Name: RequestBusiness RequestBusiness_RequestId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RequestBusiness"
    ADD CONSTRAINT "RequestBusiness_RequestId_fkey" FOREIGN KEY ("RequestId") REFERENCES public."Request"("RequestId");


--
-- TOC entry 5015 (class 2606 OID 33182)
-- Name: RequestClient RequestClient_RegionId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RequestClient"
    ADD CONSTRAINT "RequestClient_RegionId_fkey" FOREIGN KEY ("RegionId") REFERENCES public."Region"("RegionId");


--
-- TOC entry 5020 (class 2606 OID 33225)
-- Name: RequestClosed RequestClosed_RequestId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RequestClosed"
    ADD CONSTRAINT "RequestClosed_RequestId_fkey" FOREIGN KEY ("RequestId") REFERENCES public."Request"("RequestId");


--
-- TOC entry 5021 (class 2606 OID 33230)
-- Name: RequestClosed RequestClosed_RequestStatusLogId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RequestClosed"
    ADD CONSTRAINT "RequestClosed_RequestStatusLogId_fkey" FOREIGN KEY ("RequestStatusLogId") REFERENCES public."RequestStatusLog"("RequestStatusLogId");


--
-- TOC entry 5022 (class 2606 OID 33247)
-- Name: RequestConcierge RequestConcierge_ConciergeId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RequestConcierge"
    ADD CONSTRAINT "RequestConcierge_ConciergeId_fkey" FOREIGN KEY ("ConciergeId") REFERENCES public."Concierge"("ConciergeId");


--
-- TOC entry 5023 (class 2606 OID 33242)
-- Name: RequestConcierge RequestConcierge_RequestId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RequestConcierge"
    ADD CONSTRAINT "RequestConcierge_RequestId_fkey" FOREIGN KEY ("RequestId") REFERENCES public."Request"("RequestId");


--
-- TOC entry 5024 (class 2606 OID 33266)
-- Name: RequestNotes RequestNotes_CreatedBy_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RequestNotes"
    ADD CONSTRAINT "RequestNotes_CreatedBy_fkey" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- TOC entry 5025 (class 2606 OID 33271)
-- Name: RequestNotes RequestNotes_ModifiedBy_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RequestNotes"
    ADD CONSTRAINT "RequestNotes_ModifiedBy_fkey" FOREIGN KEY ("ModifiedBy") REFERENCES public."AspNetUsers"("Id");


--
-- TOC entry 5026 (class 2606 OID 33261)
-- Name: RequestNotes RequestNotes_RequestId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RequestNotes"
    ADD CONSTRAINT "RequestNotes_RequestId_fkey" FOREIGN KEY ("RequestId") REFERENCES public."Request"("RequestId");


--
-- TOC entry 5016 (class 2606 OID 33206)
-- Name: RequestStatusLog RequestStatusLog_AdminId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RequestStatusLog"
    ADD CONSTRAINT "RequestStatusLog_AdminId_fkey" FOREIGN KEY ("AdminId") REFERENCES public."Admin"("AdminId");


--
-- TOC entry 5017 (class 2606 OID 33201)
-- Name: RequestStatusLog RequestStatusLog_PhysicianId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RequestStatusLog"
    ADD CONSTRAINT "RequestStatusLog_PhysicianId_fkey" FOREIGN KEY ("PhysicianId") REFERENCES public."Physician"("PhysicianId");


--
-- TOC entry 5018 (class 2606 OID 33196)
-- Name: RequestStatusLog RequestStatusLog_RequestId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RequestStatusLog"
    ADD CONSTRAINT "RequestStatusLog_RequestId_fkey" FOREIGN KEY ("RequestId") REFERENCES public."Request"("RequestId");


--
-- TOC entry 5019 (class 2606 OID 33211)
-- Name: RequestStatusLog RequestStatusLog_TransToPhysicianId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RequestStatusLog"
    ADD CONSTRAINT "RequestStatusLog_TransToPhysicianId_fkey" FOREIGN KEY ("TransToPhysicianId") REFERENCES public."Physician"("PhysicianId");


--
-- TOC entry 5027 (class 2606 OID 33295)
-- Name: RequestWiseFile RequestWiseFile_AdminId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RequestWiseFile"
    ADD CONSTRAINT "RequestWiseFile_AdminId_fkey" FOREIGN KEY ("AdminId") REFERENCES public."Admin"("AdminId");


--
-- TOC entry 5028 (class 2606 OID 33290)
-- Name: RequestWiseFile RequestWiseFile_PhysicianId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RequestWiseFile"
    ADD CONSTRAINT "RequestWiseFile_PhysicianId_fkey" FOREIGN KEY ("PhysicianId") REFERENCES public."Physician"("PhysicianId");


--
-- TOC entry 5029 (class 2606 OID 33285)
-- Name: RequestWiseFile RequestWiseFile_RequestId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RequestWiseFile"
    ADD CONSTRAINT "RequestWiseFile_RequestId_fkey" FOREIGN KEY ("RequestId") REFERENCES public."Request"("RequestId");


--
-- TOC entry 5010 (class 2606 OID 33146)
-- Name: Request Request_PhysicianId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Request"
    ADD CONSTRAINT "Request_PhysicianId_fkey" FOREIGN KEY ("PhysicianId") REFERENCES public."Physician"("PhysicianId");


--
-- TOC entry 5011 (class 2606 OID 33141)
-- Name: Request Request_UserId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Request"
    ADD CONSTRAINT "Request_UserId_fkey" FOREIGN KEY ("UserId") REFERENCES public."User"("UserId");


--
-- TOC entry 4993 (class 2606 OID 32988)
-- Name: RoleMenu RoleMenu_MenuId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RoleMenu"
    ADD CONSTRAINT "RoleMenu_MenuId_fkey" FOREIGN KEY ("MenuId") REFERENCES public."Menu"("MenuId");


--
-- TOC entry 4994 (class 2606 OID 32983)
-- Name: RoleMenu RoleMenu_RoleId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RoleMenu"
    ADD CONSTRAINT "RoleMenu_RoleId_fkey" FOREIGN KEY ("RoleId") REFERENCES public."Role"("RoleId");


--
-- TOC entry 4999 (class 2606 OID 33039)
-- Name: ShiftDetailRegion ShiftDetailRegion_RegionId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ShiftDetailRegion"
    ADD CONSTRAINT "ShiftDetailRegion_RegionId_fkey" FOREIGN KEY ("RegionId") REFERENCES public."Region"("RegionId");


--
-- TOC entry 5000 (class 2606 OID 33034)
-- Name: ShiftDetailRegion ShiftDetailRegion_ShiftDetailId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ShiftDetailRegion"
    ADD CONSTRAINT "ShiftDetailRegion_ShiftDetailId_fkey" FOREIGN KEY ("ShiftDetailId") REFERENCES public."ShiftDetail"("ShiftDetailId");


--
-- TOC entry 4997 (class 2606 OID 33022)
-- Name: ShiftDetail ShiftDetail_ModifiedBy_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ShiftDetail"
    ADD CONSTRAINT "ShiftDetail_ModifiedBy_fkey" FOREIGN KEY ("ModifiedBy") REFERENCES public."AspNetUsers"("Id");


--
-- TOC entry 4998 (class 2606 OID 33017)
-- Name: ShiftDetail ShiftDetail_ShiftId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ShiftDetail"
    ADD CONSTRAINT "ShiftDetail_ShiftId_fkey" FOREIGN KEY ("ShiftId") REFERENCES public."Shift"("ShiftId");


--
-- TOC entry 4995 (class 2606 OID 33005)
-- Name: Shift Shift_CreatedBy_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Shift"
    ADD CONSTRAINT "Shift_CreatedBy_fkey" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- TOC entry 4996 (class 2606 OID 33000)
-- Name: Shift Shift_PhysicianId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Shift"
    ADD CONSTRAINT "Shift_PhysicianId_fkey" FOREIGN KEY ("PhysicianId") REFERENCES public."Physician"("PhysicianId");


--
-- TOC entry 5001 (class 2606 OID 33062)
-- Name: User User_AspNetUserId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."User"
    ADD CONSTRAINT "User_AspNetUserId_fkey" FOREIGN KEY ("AspNetUserId") REFERENCES public."AspNetUsers"("Id");


--
-- TOC entry 5002 (class 2606 OID 33067)
-- Name: User User_CreatedBy_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."User"
    ADD CONSTRAINT "User_CreatedBy_fkey" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- TOC entry 5003 (class 2606 OID 33072)
-- Name: User User_ModifiedBy_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."User"
    ADD CONSTRAINT "User_ModifiedBy_fkey" FOREIGN KEY ("ModifiedBy") REFERENCES public."AspNetUsers"("Id");


--
-- TOC entry 5030 (class 2606 OID 49177)
-- Name: EncounterForm fk_encounter_request; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."EncounterForm"
    ADD CONSTRAINT fk_encounter_request FOREIGN KEY ("RequestId") REFERENCES public."Request"("RequestId");


--
-- TOC entry 5012 (class 2606 OID 33301)
-- Name: Request fk_request_requestclient; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Request"
    ADD CONSTRAINT fk_request_requestclient FOREIGN KEY ("RequestClientId") REFERENCES public."RequestClient"("RequestClientId");


-- Completed on 2024-03-28 19:21:59

--
-- PostgreSQL database dump complete
--

