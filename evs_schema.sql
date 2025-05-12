--
-- PostgreSQL database dump
--

-- Dumped from database version 17.4
-- Dumped by pg_dump version 17.4

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 6 (class 2615 OID 16657)
-- Name: evs; Type: SCHEMA; Schema: -
--

DROP SCHEMA IF EXISTS evs CASCADE;

CREATE SCHEMA IF NOT EXISTS evs;


SET default_table_access_method = heap;

--
-- TOC entry 218 (class 1259 OID 16658)
-- Name: cars; Type: TABLE; Schema: evs
--

CREATE TABLE evs.cars (
    id bigint NOT NULL,
    uuid uuid NOT NULL,
    sid character varying(31) NOT NULL,
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone,
    vin_1_10 character varying(10) NOT NULL,
    county character varying(127),
    city character varying(255),
    state character varying(2) NOT NULL,
    zip_code character varying(5),
    model_id integer NOT NULL,
    legislative_district character varying(31),
    dol_vehicle_id character varying(31) NOT NULL,
    vehicle_location point,
    electric_utility character varying(255),
    census_tract_2020 character varying(127),
    counties bigint,
    districts bigint,
    legislative_district_boundary bigint
);


--
-- TOC entry 4821 (class 0 OID 0)
-- Dependencies: 218
-- Name: COLUMN cars.vin_1_10; Type: COMMENT; Schema: evs
--

COMMENT ON COLUMN evs.cars.vin_1_10 IS 'first 10 characters of a VIN (a 17 character code)';


--
-- TOC entry 4822 (class 0 OID 0)
-- Dependencies: 218
-- Name: COLUMN cars.dol_vehicle_id; Type: COMMENT; Schema: evs
--

COMMENT ON COLUMN evs.cars.dol_vehicle_id IS 'Cannot assume pure numeric despite it being apparent';


--
-- TOC entry 4823 (class 0 OID 0)
-- Dependencies: 218
-- Name: COLUMN cars.census_tract_2020; Type: COMMENT; Schema: evs
--

COMMENT ON COLUMN evs.cars.census_tract_2020 IS 'Apparent numeric sequence but no spec means we treat it like a string';


--
-- TOC entry 4824 (class 0 OID 0)
-- Dependencies: 218
-- Name: COLUMN cars.counties; Type: COMMENT; Schema: evs
--

COMMENT ON COLUMN evs.cars.counties IS 'Assumed this is a choropleth indicator';


--
-- TOC entry 4825 (class 0 OID 0)
-- Dependencies: 218
-- Name: COLUMN cars.districts; Type: COMMENT; Schema: evs
--

COMMENT ON COLUMN evs.cars.districts IS 'Assumed this is a choropleth indicator';


--
-- TOC entry 4826 (class 0 OID 0)
-- Dependencies: 218
-- Name: COLUMN cars.legislative_district_boundary; Type: COMMENT; Schema: evs
--

COMMENT ON COLUMN evs.cars.legislative_district_boundary IS 'Assumed this is a choropleth indicator';


--
-- TOC entry 219 (class 1259 OID 16663)
-- Name: cars_id_seq1; Type: SEQUENCE; Schema: evs
--

CREATE SEQUENCE evs.cars_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4827 (class 0 OID 0)
-- Dependencies: 219
-- Name: cars_id_seq1; Type: SEQUENCE OWNED BY; Schema: evs
--

ALTER SEQUENCE evs.cars_id_seq OWNED BY evs.cars.id;



--
-- TOC entry 221 (class 1259 OID 16665)
-- Name: makes; Type: TABLE; Schema: evs
--

CREATE TABLE evs.makes (
    id integer NOT NULL,
    manufacturer character varying(255) NOT NULL
);


--
-- TOC entry 222 (class 1259 OID 16668)
-- Name: makes_id_seq; Type: SEQUENCE; Schema: evs
--

CREATE SEQUENCE evs.makes_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4829 (class 0 OID 0)
-- Dependencies: 222
-- Name: makes_id_seq; Type: SEQUENCE OWNED BY; Schema: evs
--

ALTER SEQUENCE evs.makes_id_seq OWNED BY evs.makes.id;


--
-- TOC entry 223 (class 1259 OID 16669)
-- Name: models; Type: TABLE; Schema: evs
--

CREATE TABLE evs.models (
    id integer NOT NULL,
    make_id integer NOT NULL,
    model_name character varying(255) NOT NULL,
    model_year integer NOT NULL,
    ev_type integer NOT NULL,
    cafv_type integer NOT NULL,
    electric_range integer DEFAULT 0 NOT NULL,
    base_msrp numeric(10,2) DEFAULT 0 NOT NULL
);



--
-- TOC entry 220 (class 1259 OID 16664)
-- Name: cars_model_seq; Type: SEQUENCE; Schema: evs
--

CREATE SEQUENCE evs.models_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4828 (class 0 OID 0)
-- Dependencies: 220
-- Name: cars_model_seq; Type: SEQUENCE OWNED BY; Schema: evs
--

ALTER SEQUENCE evs.models_id_seq OWNED BY evs.models.id;
--
-- TOC entry 4652 (class 2604 OID 16674)
-- Name: cars id; Type: DEFAULT; Schema: evs
--

ALTER TABLE ONLY evs.cars ALTER COLUMN id SET DEFAULT nextval('evs.cars_id_seq'::regclass);


--
-- TOC entry 4653 (class 2604 OID 16675)
-- Name: model id; Type: DEFAULT; Schema: evs
--

ALTER TABLE ONLY evs.models ALTER COLUMN id SET DEFAULT nextval('evs.models_id_seq'::regclass);


--
-- TOC entry 4654 (class 2604 OID 16676)
-- Name: makes id; Type: DEFAULT; Schema: evs
--

ALTER TABLE ONLY evs.makes ALTER COLUMN id SET DEFAULT nextval('evs.makes_id_seq'::regclass);


--
-- TOC entry 4658 (class 2606 OID 16678)
-- Name: cars cars_pkey1; Type: CONSTRAINT; Schema: evs
--

ALTER TABLE ONLY evs.cars
    ADD CONSTRAINT cars_pkey1 PRIMARY KEY (id);


--
-- TOC entry 4662 (class 2606 OID 16680)
-- Name: makes makes_pkey; Type: CONSTRAINT; Schema: evs
--

ALTER TABLE ONLY evs.makes
    ADD CONSTRAINT makes_pkey PRIMARY KEY (id);


--
-- TOC entry 4664 (class 2606 OID 16682)
-- Name: makes manufacturer_unique; Type: CONSTRAINT; Schema: evs
--

ALTER TABLE ONLY evs.makes
    ADD CONSTRAINT manufacturer_unique UNIQUE (manufacturer);


--
-- TOC entry 4666 (class 2606 OID 16684)
-- Name: models model_name_unique; Type: CONSTRAINT; Schema: evs
--

ALTER TABLE ONLY evs.models
    ADD CONSTRAINT model_name_unique UNIQUE (make_id, model_name);


--
-- TOC entry 4668 (class 2606 OID 16686)
-- Name: models models_pkey; Type: CONSTRAINT; Schema: evs
--

ALTER TABLE ONLY evs.models
    ADD CONSTRAINT models_pkey PRIMARY KEY (id);


--
-- TOC entry 4660 (class 2606 OID 16688)
-- Name: cars uuid_unique; Type: CONSTRAINT; Schema: evs
--

ALTER TABLE ONLY evs.cars
    ADD CONSTRAINT uuid_unique UNIQUE (uuid);


--
-- TOC entry 4670 (class 2606 OID 16689)
-- Name: models fk_make_id; Type: FK CONSTRAINT; Schema: evs
--

ALTER TABLE ONLY evs.models
    ADD CONSTRAINT fk_make_id FOREIGN KEY (make_id) REFERENCES evs.makes(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 4669 (class 2606 OID 16694)
-- Name: cars fk_model_id; Type: FK CONSTRAINT; Schema: evs
--

ALTER TABLE ONLY evs.cars
    ADD CONSTRAINT fk_model_id FOREIGN KEY (model_id) REFERENCES evs.models(id) NOT VALID;


--
-- PostgreSQL database dump complete
--

