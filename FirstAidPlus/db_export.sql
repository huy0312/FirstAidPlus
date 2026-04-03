--
-- PostgreSQL database dump
--

\restrict QW4DDy55SXhuRdKDvB6qLMFC05ubjiWagOVHhzlqd7YSXFCRqbETJklEV7BDgAV

-- Dumped from database version 18.1
-- Dumped by pg_dump version 18.1

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

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


--
-- Name: carts; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.carts (
    id integer NOT NULL,
    user_id integer NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


--
-- Name: carts_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.carts_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: carts_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.carts_id_seq OWNED BY public.carts.id;


--
-- Name: comment_reactions; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.comment_reactions (
    id integer NOT NULL,
    comment_id integer NOT NULL,
    user_id integer NOT NULL,
    reaction_type character varying(10) DEFAULT '👍'::character varying NOT NULL,
    created_at timestamp without time zone DEFAULT now()
);


--
-- Name: comment_reactions_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.comment_reactions_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: comment_reactions_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.comment_reactions_id_seq OWNED BY public.comment_reactions.id;


--
-- Name: course_lessons; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.course_lessons (
    id integer NOT NULL,
    syllabus_id integer NOT NULL,
    title character varying(255) NOT NULL,
    type character varying(50) DEFAULT 'Reading'::character varying,
    content text,
    video_url character varying(500),
    duration integer DEFAULT 0,
    order_index integer DEFAULT 0,
    created_at timestamp without time zone DEFAULT now(),
    description text
);


--
-- Name: course_lessons_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.course_lessons_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: course_lessons_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.course_lessons_id_seq OWNED BY public.course_lessons.id;


--
-- Name: course_objectives; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.course_objectives (
    id integer NOT NULL,
    course_id integer NOT NULL,
    content text NOT NULL
);


--
-- Name: course_objectives_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.course_objectives_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: course_objectives_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.course_objectives_id_seq OWNED BY public.course_objectives.id;


--
-- Name: course_syllabus; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.course_syllabus (
    id integer NOT NULL,
    course_id integer NOT NULL,
    title character varying(255) NOT NULL,
    duration character varying(50),
    lesson_count integer DEFAULT 0
);


--
-- Name: course_syllabus_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.course_syllabus_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: course_syllabus_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.course_syllabus_id_seq OWNED BY public.course_syllabus.id;


--
-- Name: courses; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.courses (
    id integer NOT NULL,
    title character varying(200) NOT NULL,
    description text,
    certificate_name character varying(200),
    image_url character varying(255),
    is_popular boolean DEFAULT false,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    video_url character varying(255),
    training_image_url character varying(255),
    instructor_id integer,
    is_active boolean DEFAULT true,
    category text
);


--
-- Name: courses_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.courses_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: courses_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.courses_id_seq OWNED BY public.courses.id;


--
-- Name: enrollments; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.enrollments (
    id integer NOT NULL,
    user_id integer NOT NULL,
    course_id integer NOT NULL,
    enrolled_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    status character varying(50) DEFAULT 'Active'::character varying,
    amount numeric(18,2) NOT NULL
);


--
-- Name: enrollments_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.enrollments_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: enrollments_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.enrollments_id_seq OWNED BY public.enrollments.id;


--
-- Name: family_course_categories; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.family_course_categories (
    id integer NOT NULL,
    title character varying(255) NOT NULL,
    description text,
    icon_url text,
    color_hex character varying(50)
);


--
-- Name: family_course_categories_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.family_course_categories_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: family_course_categories_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.family_course_categories_id_seq OWNED BY public.family_course_categories.id;


--
-- Name: feedbacks; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.feedbacks (
    id integer NOT NULL,
    user_id integer NOT NULL,
    course_id integer NOT NULL,
    rating integer NOT NULL,
    comment text NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL
);


--
-- Name: feedbacks_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.feedbacks_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: feedbacks_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.feedbacks_id_seq OWNED BY public.feedbacks.id;


--
-- Name: game_category_experts; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.game_category_experts (
    id integer NOT NULL,
    expert_id integer NOT NULL,
    category_id integer NOT NULL,
    assigned_at timestamp without time zone DEFAULT now() NOT NULL
);


--
-- Name: game_category_experts_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.game_category_experts_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: game_category_experts_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.game_category_experts_id_seq OWNED BY public.game_category_experts.id;


--
-- Name: game_options; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.game_options (
    id integer NOT NULL,
    situation_id integer NOT NULL,
    option_text text NOT NULL,
    is_correct boolean DEFAULT false NOT NULL,
    explanation text,
    points integer DEFAULT 0 NOT NULL,
    icon_url text
);


--
-- Name: game_options_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.game_options_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: game_options_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.game_options_id_seq OWNED BY public.game_options.id;


--
-- Name: game_situations; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.game_situations (
    id integer NOT NULL,
    category_id integer NOT NULL,
    title character varying(255) NOT NULL,
    character_context text,
    situation_description text,
    question text NOT NULL,
    image_url text
);


--
-- Name: game_situations_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.game_situations_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: game_situations_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.game_situations_id_seq OWNED BY public.game_situations.id;


--
-- Name: lesson_comments; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.lesson_comments (
    id integer NOT NULL,
    lesson_id integer NOT NULL,
    user_id integer NOT NULL,
    content text NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    parent_id integer
);


--
-- Name: lesson_comments_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.lesson_comments_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: lesson_comments_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.lesson_comments_id_seq OWNED BY public.lesson_comments.id;


--
-- Name: lesson_notes; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.lesson_notes (
    id integer NOT NULL,
    lesson_id integer NOT NULL,
    user_id integer NOT NULL,
    content text NOT NULL,
    video_timestamp double precision,
    created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp with time zone
);


--
-- Name: lesson_notes_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.lesson_notes_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: lesson_notes_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.lesson_notes_id_seq OWNED BY public.lesson_notes.id;


--
-- Name: medical_records; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.medical_records (
    id integer NOT NULL,
    user_id integer NOT NULL,
    condition_name text NOT NULL,
    description text,
    year_diagnosed integer NOT NULL,
    created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
);


--
-- Name: medical_records_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public.medical_records ALTER COLUMN id ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public.medical_records_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: messages; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.messages (
    id integer NOT NULL,
    sender_id integer NOT NULL,
    receiver_id integer NOT NULL,
    content text NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


--
-- Name: messages_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.messages_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: messages_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.messages_id_seq OWNED BY public.messages.id;


--
-- Name: notifications; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.notifications (
    id integer NOT NULL,
    user_id integer NOT NULL,
    title character varying(255) NOT NULL,
    message text NOT NULL,
    link character varying(500),
    is_read boolean DEFAULT false NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
);


--
-- Name: notifications_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.notifications_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: notifications_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.notifications_id_seq OWNED BY public.notifications.id;


--
-- Name: plan_courses; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.plan_courses (
    plan_id integer NOT NULL,
    course_id integer NOT NULL
);


--
-- Name: plans; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.plans (
    id integer NOT NULL,
    name character varying(50) NOT NULL,
    price numeric(18,2) NOT NULL,
    description text,
    features text,
    duration_months integer DEFAULT 12,
    durationvalue integer DEFAULT 1 NOT NULL,
    durationunit character varying(20) DEFAULT 'Month'::character varying NOT NULL,
    duration_value integer DEFAULT 0,
    duration_unit character varying(50) DEFAULT 'Month'::character varying
);


--
-- Name: plans_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.plans_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: plans_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.plans_id_seq OWNED BY public.plans.id;


--
-- Name: qualifications; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.qualifications (
    id integer NOT NULL,
    title character varying(200) NOT NULL,
    description text,
    certificate_url text,
    issued_at timestamp without time zone NOT NULL,
    user_id integer NOT NULL,
    status character varying(50) DEFAULT 'Pending'::character varying,
    admin_comment text
);


--
-- Name: qualifications_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.qualifications_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: qualifications_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.qualifications_id_seq OWNED BY public.qualifications.id;


--
-- Name: roles; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.roles (
    id integer NOT NULL,
    role_name character varying(50) NOT NULL
);


--
-- Name: roles_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.roles_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: roles_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.roles_id_seq OWNED BY public.roles.id;


--
-- Name: settings; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.settings (
    key character varying(100) NOT NULL,
    value text NOT NULL,
    description text,
    "group" character varying(50) DEFAULT 'General'::character varying
);


--
-- Name: testimonials; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.testimonials (
    id integer NOT NULL,
    student_name character varying(100) NOT NULL,
    student_role character varying(100),
    content text,
    rating integer,
    CONSTRAINT testimonials_rating_check CHECK (((rating >= 1) AND (rating <= 5)))
);


--
-- Name: testimonials_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.testimonials_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: testimonials_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.testimonials_id_seq OWNED BY public.testimonials.id;


--
-- Name: transactions; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.transactions (
    id integer NOT NULL,
    user_id integer NOT NULL,
    plan_id integer NOT NULL,
    amount numeric(18,2) NOT NULL,
    order_description text,
    vnp_txn_ref character varying(100),
    vnp_transaction_no character varying(100),
    status character varying(50) DEFAULT 'Pending'::character varying,
    created_at timestamp without time zone DEFAULT now(),
    momo_order_id text,
    momo_trans_id text,
    payment_method text DEFAULT 'VnPay'::text NOT NULL
);


--
-- Name: transactions_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.transactions_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: transactions_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.transactions_id_seq OWNED BY public.transactions.id;


--
-- Name: user_game_progress; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.user_game_progress (
    id integer NOT NULL,
    user_id integer NOT NULL,
    situation_id integer NOT NULL,
    is_completed boolean DEFAULT false NOT NULL,
    score_earned integer DEFAULT 0 NOT NULL,
    completed_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
);


--
-- Name: user_game_progress_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.user_game_progress_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: user_game_progress_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.user_game_progress_id_seq OWNED BY public.user_game_progress.id;


--
-- Name: user_lesson_progress; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.user_lesson_progress (
    id integer NOT NULL,
    user_id integer NOT NULL,
    lesson_id integer NOT NULL,
    time_spent_seconds integer DEFAULT 0 NOT NULL,
    is_completed boolean DEFAULT false NOT NULL,
    last_accessed timestamp without time zone DEFAULT now() NOT NULL
);


--
-- Name: user_lesson_progress_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.user_lesson_progress_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: user_lesson_progress_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.user_lesson_progress_id_seq OWNED BY public.user_lesson_progress.id;


--
-- Name: user_subscriptions; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.user_subscriptions (
    id integer NOT NULL,
    user_id integer NOT NULL,
    plan_id integer NOT NULL,
    start_date timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    end_date timestamp without time zone,
    status character varying(20) DEFAULT 'Active'::character varying
);


--
-- Name: user_subscriptions_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.user_subscriptions_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: user_subscriptions_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.user_subscriptions_id_seq OWNED BY public.user_subscriptions.id;


--
-- Name: users; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.users (
    id integer NOT NULL,
    username character varying(50) NOT NULL,
    email character varying(100) NOT NULL,
    password_hash character varying(255) NOT NULL,
    full_name character varying(100),
    role_id integer NOT NULL,
    phone character varying(20),
    address text,
    bio text,
    avatar_url character varying(255),
    reset_token character varying(255),
    reset_token_expiry timestamp without time zone,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    is_email_confirmed boolean DEFAULT false,
    email_confirmation_token character varying(255)
);


--
-- Name: users_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.users_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: users_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.users_id_seq OWNED BY public.users.id;


--
-- Name: carts id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.carts ALTER COLUMN id SET DEFAULT nextval('public.carts_id_seq'::regclass);


--
-- Name: comment_reactions id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.comment_reactions ALTER COLUMN id SET DEFAULT nextval('public.comment_reactions_id_seq'::regclass);


--
-- Name: course_lessons id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.course_lessons ALTER COLUMN id SET DEFAULT nextval('public.course_lessons_id_seq'::regclass);


--
-- Name: course_objectives id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.course_objectives ALTER COLUMN id SET DEFAULT nextval('public.course_objectives_id_seq'::regclass);


--
-- Name: course_syllabus id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.course_syllabus ALTER COLUMN id SET DEFAULT nextval('public.course_syllabus_id_seq'::regclass);


--
-- Name: courses id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.courses ALTER COLUMN id SET DEFAULT nextval('public.courses_id_seq'::regclass);


--
-- Name: enrollments id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.enrollments ALTER COLUMN id SET DEFAULT nextval('public.enrollments_id_seq'::regclass);


--
-- Name: family_course_categories id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.family_course_categories ALTER COLUMN id SET DEFAULT nextval('public.family_course_categories_id_seq'::regclass);


--
-- Name: feedbacks id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.feedbacks ALTER COLUMN id SET DEFAULT nextval('public.feedbacks_id_seq'::regclass);


--
-- Name: game_category_experts id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.game_category_experts ALTER COLUMN id SET DEFAULT nextval('public.game_category_experts_id_seq'::regclass);


--
-- Name: game_options id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.game_options ALTER COLUMN id SET DEFAULT nextval('public.game_options_id_seq'::regclass);


--
-- Name: game_situations id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.game_situations ALTER COLUMN id SET DEFAULT nextval('public.game_situations_id_seq'::regclass);


--
-- Name: lesson_comments id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.lesson_comments ALTER COLUMN id SET DEFAULT nextval('public.lesson_comments_id_seq'::regclass);


--
-- Name: lesson_notes id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.lesson_notes ALTER COLUMN id SET DEFAULT nextval('public.lesson_notes_id_seq'::regclass);


--
-- Name: messages id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.messages ALTER COLUMN id SET DEFAULT nextval('public.messages_id_seq'::regclass);


--
-- Name: notifications id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.notifications ALTER COLUMN id SET DEFAULT nextval('public.notifications_id_seq'::regclass);


--
-- Name: plans id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.plans ALTER COLUMN id SET DEFAULT nextval('public.plans_id_seq'::regclass);


--
-- Name: qualifications id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.qualifications ALTER COLUMN id SET DEFAULT nextval('public.qualifications_id_seq'::regclass);


--
-- Name: roles id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.roles ALTER COLUMN id SET DEFAULT nextval('public.roles_id_seq'::regclass);


--
-- Name: testimonials id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.testimonials ALTER COLUMN id SET DEFAULT nextval('public.testimonials_id_seq'::regclass);


--
-- Name: transactions id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.transactions ALTER COLUMN id SET DEFAULT nextval('public.transactions_id_seq'::regclass);


--
-- Name: user_game_progress id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.user_game_progress ALTER COLUMN id SET DEFAULT nextval('public.user_game_progress_id_seq'::regclass);


--
-- Name: user_lesson_progress id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.user_lesson_progress ALTER COLUMN id SET DEFAULT nextval('public.user_lesson_progress_id_seq'::regclass);


--
-- Name: user_subscriptions id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.user_subscriptions ALTER COLUMN id SET DEFAULT nextval('public.user_subscriptions_id_seq'::regclass);


--
-- Name: users id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.users ALTER COLUMN id SET DEFAULT nextval('public.users_id_seq'::regclass);


--
-- Data for Name: __EFMigrationsHistory; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."__EFMigrationsHistory" ("MigrationId", "ProductVersion") FROM stdin;
\.


--
-- Data for Name: carts; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.carts (id, user_id, created_at) FROM stdin;
\.


--
-- Data for Name: comment_reactions; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.comment_reactions (id, comment_id, user_id, reaction_type, created_at) FROM stdin;
1	3	18	❤️	2026-03-10 04:19:15.288984
2	7	18	❤️	2026-03-10 04:41:42.560037
3	16	18	😢	2026-03-14 05:05:34.180751
\.


--
-- Data for Name: course_lessons; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.course_lessons (id, syllabus_id, title, type, content, video_url, duration, order_index, created_at, description) FROM stdin;
3	37	Bài 3: Bài tập tình huống thực tế số 1	Reading	<h3>Tình huống xử lý hiện trường</h3>\n        <p>Bạn thấy một người thợ điện ngã gục ngay bên cạnh tuabin điện của tòa nhà...</p>\n        <div class="p-4 bg-light border rounded mt-3 shadow-sm">\n            <h5 class="text-danger mb-3"><i class="fas fa-question-circle me-2"></i>Câu hỏi: Hành động ĐẦU TIÊN bạn cần làm là gì?</h5>\n            <ol class="mt-2 fs-6">\n                <li>Chạy ngay tới lay người thợ điện dậy xem họ còn tỉnh không.</li>\n                <li>Gọi cấp cứu 115 và ngồi đợi.</li>\n                <li><strong>Ngắt cầu dao điện tổng trước khi tiếp cận hiện trường (Nguyên tắc Danger).</strong></li>\n                <li>Bắt đầu hô hấp nhân tạo lập tức.</li>\n            </ol>\n            <div class="alert alert-success mt-3 mb-0 border-success"><i class="fas fa-check-circle me-1"></i> <strong>Giải thích:</strong> Đáp án đúng là số 3. Luôn đảm bảo an toàn cho bản thân trước tiên. Bỏ qua bước D (Danger), bạn có thể trở thành nạn nhân thứ hai.</div>\n        </div>	\N	15	3	2026-03-06 14:42:08.749266	\N
4	38	Bài 4: Khái niệm Chuỗi Cứu Sống (Chain of Survival)	Reading	<p>Chuỗi cứu sống do AHA khuyến cáo giúp tăng cường tỷ lệ sống sót trong ngưng tim lên đến 70%. Bao gồm 5 mắt xích quan trọng:</p>\n        <ol>\n            <li>GỌI CẤP CỨU sớm (115)</li>\n            <li>THỰC HIỆN CPR sớm (Nhấn mạnh vào lồng ngực)</li>\n            <li>DÙNG AED sớm (Máy khử rung sốc điện)</li>\n            <li>CHĂM SÓC Y TẾ cao cấp (Bên trong xe cứu thương/bệnh viện)</li>\n            <li>PHỤC HỒI sau viện.</li>\n        </ol>	\N	10	1	2026-03-06 14:42:08.749266	\N
5	38	Bài 5: Bắt đầu kỹ thuật ép tim lồng ngực (CPR Người lớn)	Video	\N	https://www.youtube.com/embed/-YyP02I-N_A	35	2	2026-03-06 14:42:08.749266	\N
6	38	Bài 6: Vận hành máy AED tự động hóa an toàn	Video	\N	https://www.youtube.com/embed/1wq-N0gA2D4	25	3	2026-03-06 14:42:08.749266	\N
7	38	Bài 7: Trắc nghiệm Hệ Thống: Ký ức ép tim	Reading	<div class="card my-3 shadow-sm border-0">\n            <div class="card-header bg-danger text-white fw-bold fs-5"><i class="fas fa-list-ul me-2"></i> Bài Tập Ôn Luyện Nhanh</div>\n            <div class="card-body bg-light">\n                <h6 class="fw-bold text-dark mt-2">Câu 1: Tần số ép tim chuẩn cho người lớn?</h6>\n                <ul class="list-unstyled mb-3 ms-3">\n                    <li><i class="far fa-circle me-2"></i>A. 60 - 80 lần/phút</li>\n                    <li class="text-danger fw-bold"><i class="far fa-check-circle me-2"></i>B. 100 - 120 lần/phút</li>\n                    <li><i class="far fa-circle me-2"></i>C. 80 - 100 lần/phút</li>\n                </ul>\n                \n                <h6 class="fw-bold text-dark">Câu 2: Độ sâu ép tim cần thiết đối với người trưởng thành?</h6>\n                <ul class="list-unstyled mb-3 ms-3">\n                    <li><i class="far fa-circle me-2"></i>A. Khoảng 3 cm</li>\n                    <li class="text-danger fw-bold"><i class="far fa-check-circle me-2"></i>B. Khoảng 5 cm (không quá 6 cm)</li>\n                    <li><i class="far fa-circle me-2"></i>C. Sâu hết cỡ có thể</li>\n                </ul>\n                \n                <h6 class="fw-bold text-dark">Câu 3: Quy tắc kết hợp giữa ép tim và thổi ngạt chuẩn AHA:</h6>\n                <ul class="list-unstyled mb-0 ms-3">\n                    <li><i class="far fa-circle me-2"></i>A. 15 lần ép tim, 2 lần thổi ngạt</li>\n                    <li class="text-danger fw-bold"><i class="far fa-check-circle me-2"></i>B. 30 lần ép tim, 2 lần thổi ngạt</li>\n                    <li><i class="far fa-circle me-2"></i>C. 30 ép tim, 5 lần thổi ngạt</li>\n                </ul>\n            </div>\n        </div>	\N	20	4	2026-03-06 14:42:08.749266	\N
8	39	Bài 8: Trị liệu nghẹt thở với nghiệm pháp Heimlich	Video	\N	https://www.youtube.com/embed/F_fP4qHXYvE	30	1	2026-03-06 14:42:08.749266	\N
9	39	Bài 9: Xử lý Vết Thương Chảy Máu Nhiều	Reading	<h3>Phác đồ cầm máu sinh tử:</h3>\n        <ol class="fs-6 mt-3 lh-lg">\n            <li>Mặc đồ bảo hộ, đeo găng tay y tế (Nếu có thời gian chuẩn bị).</li>\n            <li>Lấy gạc đắp trực tiếp lên vết thương, dùng lòng bàn tay tì và <strong>ép lực rất mạnh</strong> xuống để bịt áp lực của mạch đứt.</li>\n            <li>Không gỡ lớp gạc đang thấm máu nếu máu vẫn rịn ra, tiếp tục đặt gạc mới chồng lên lớp gạc cũ.</li>\n            <li>Quấn băng thun ép chặt lại để duy trì sức ép. Hãy nâng cao cẳng tay/cẳng chân lên cao hơn tim nếu loại trừ khả năng gãy xương.</li>\n            <li>Chỉ dùng GARO cho tay/chân khi đứt lìa chi, hoặc máu phun tia dữ dội không thể ép dừng. Ghi chú rõ giờ sử dụng Garo.</li>\n        </ol>	\N	25	2	2026-03-06 14:42:08.749266	\N
10	39	Bài 10: Xử lý vết bỏng nước sôi / Bỏng nhiệt	Reading	<p class="fs-5 pb-2 text-danger"><b><i class="fas fa-fire me-2"></i>Quy tắc Vàng "20 Phút Dưới Vòi Nước Mát":</b></p>\n        <p>Để vùng da bị bỏng ngâm dưới dòng nước mát xả cực nhẹ từ <strong>15 đến 20 phút</strong> ngay lập tức. Nước mát có tác dụng tải nhiệt và khóa lại tổn thương không ăn sâu thêm các mô thịt. Dùng màng bọc bọc lỏng lẻo nếu cần bảo quản khi chuyển viện.</p>\n        <div class="alert alert-danger"><i class="fas fa-exclamation-triangle me-2"></i><strong>Tối Kỵ:</strong> Tuyệt đối không bôi mỡ trăn, bơ, nước mắm, kem đánh răng hay chọc thủng bọng nước để tránh hoại tử.</div>	\N	15	3	2026-03-06 14:42:08.749266	\N
1	37	Bài 1: Nguyên tắc 3C (Check - Call - Care)	Video	\N	https://www.youtube.com/watch?v=-KquSjRH4r0	1	1	2026-03-06 14:42:08.749266	\N
2	37	Bài 2: Đánh giá nhanh tình trạng nạn nhân (DRABC)	Reading	<h3>Quy trình DRABC</h3>\n        <p>Thuật ngữ DRABC là kim chỉ nam để đánh giá và xử lý nạn nhân trước khi y tế đến:</p>\n        <ul>\n            <li><b>D - Danger:</b> Đảm bảo hiện trường an toàn (điện, cháy nổ, giao thông).</li>\n            <li><b>R - Response:</b> Kiểm tra phản ứng (gọi to, lay nhẹ vai).</li>\n            <li><b>A - Airway:</b> Mở thông đường thở (ngửa đầu, nâng cằm).</li>\n            <li><b>B - Breathing:</b> Áp sát tai để nghe, nhìn ngực bệnh nhân.</li>\n            <li><b>C - Circulation/CPR:</b> Nếu không thở bình thường, lập tức ép tim.</li>\n        </ul>	https://www.youtube.com/watch?v=Hsl_JUYJUoc	1	2	2026-03-06 14:42:08.749266	\N
44	52	Bài 1: Tổng quan và Lời mở đầu	Document	Tình trạng Huấn luyện Sơ cứu tại Công trường (OSHA chuẩn) là một trong những ca cấp cứu thường gặp nhất.	\N	5	0	2026-03-06 17:51:45.092952	\N
45	52	Bài 2: Tính khẩn cấp và Giờ vàng	Video	Khái niệm giờ vàng và tại sao bạn phải hành động trong phút đầu tiên.	https://www.youtube.com/watch?v=dQw4w9WgXcQ	15	1	2026-03-06 17:51:45.092952	\N
11	39	Bài 11: 📝 Kiểm Tra Mô Phỏng Nhận Chứng Chỉ	Exam	<div class="alert alert-warning mt-4 border-warning shadow-sm">\n            <h4 class="alert-heading text-center fw-bold text-dark"><i class="fas fa-award text-danger fs-3 mb-2 d-block"></i> KIỂM TRA ĐÁNH GIÁ THỰC TẾ CUỐI KHÓA</h4>\n            <p class="text-dark opacity-75 text-center px-4 mb-4">Bạn hãy đọc kỹ đoạn kịch bản lâm sàng dưới đây và nhẩm trong miệng bước ứng xử để vượt qua bài tốt nghiệp.</p>\n            <hr class="border-secondary opacity-25">\n            <p class="mb-2 fs-5 text-danger fw-bold"><i class="fas fa-exclamation-circle me-2"></i>Trường hợp 1: Tắc Đường Thở Hoàn Toàn (Choking)</p>\n            <p class="mb-3 text-dark">Một bác lớn tuổi khi ăn ở mâm cỗ cạnh bạn đột ngột lấy tay úp vào vùng cổ, môi tím tái, mặt hoảng loạn tột độ và không thể nói tiếng nào khi bạn lên tiếng hỏi.</p>\n            <div class="bg-white p-4 rounded text-dark shadow-sm lh-lg border">\n                <h6 class="fw-bold mb-3 fs-5 text-primary">Các bước hành động sinh tử theo Heimlich Maneuver:</h6>\n                <ol class="mb-0">\n                    <li>Hô hoán và nhờ đích danh 1 người xung quanh gọi lập tức số 115.</li>\n                    <li>Tiến lại từ phía sau lưng nạn nhân, chân trước lách luồn giữa hai chân họ làm đối trọng.</li>\n                    <li>Nắm chặt một bàn tay, để mép bên phần ngón cái lọt vào vùng giữa rốn và mũi ức. Bàn tay còn lại nắm bọc chặt lên nắm tay kia.</li>\n                    <li><strong>Giật thúc mạnh vô cùng dứt khoát</strong> theo hướng đè vòng trong và hất cao lên về phía hoành để ngưng ép tạo áp lực bật vật cản. Tính chu kỳ 5 nhịp/lần.</li>\n                    <li>Nếu họ ngã quỵ xuống, hãy dìu họ nằm ngửa ra sàn phẳng an toàn và <em>bắt đầu ép tim CPR ngay</em>.</li>\n                </ol>\n            </div>\n            <div class="mt-4 text-center">\n                <p class="mb-0 text-success fw-bold fs-5 mt-4"><i class="fas fa-check me-2"></i> Chúc mừng bạn đã hoàn thành khóa học và ghi nhớ bài rất xuất sắc!</p>\n            </div>\n        </div>	\N	60	4	2026-03-06 14:42:08.749266	\N
13	37	Bài 4: Kết Tổng hợp kiến thức và kết thúc chương 1	Video	\N	https://www.youtube.com/watch?v=2FzlpZqyuKg&list=RD2FzlpZqyuKg&start_radio=1	10	4	2026-03-06 08:57:02.861431	\N
17	43	Bài 1: Tổng quan và Lời mở đầu	Document	Tình trạng Cấp cứu chấn thương sọ não cơ bản là một trong những ca cấp cứu thường gặp nhất.	\N	5	0	2026-03-06 17:51:45.092952	\N
18	43	Bài 2: Tính khẩn cấp và Giờ vàng	Video	Khái niệm giờ vàng và tại sao bạn phải hành động trong phút đầu tiên.	https://www.youtube.com/watch?v=dQw4w9WgXcQ	15	1	2026-03-06 17:51:45.092952	\N
19	43	Bài 3: Chuẩn bị thiết bị cần thiết	Document	Những thứ bạn cần có trong bộ sơ cứu chuyên dụng.	\N	10	2	2026-03-06 17:51:45.092952	\N
20	44	Bài 4: Phân tích đánh giá tình trạng bệnh nhân	Video	Sử dụng thang đo DRABC để đánh giá tổng quan.	https://www.youtube.com/watch?v=dQw4w9WgXcQ	20	0	2026-03-06 17:51:45.092952	\N
21	44	Bài 5: Hướng dẫn kỹ thuật thủ công	Document	Chi tiết các bước từ 1 đến 5 để thao tác.	\N	12	1	2026-03-06 17:51:45.092952	\N
22	44	Bài 6: Phân biệt sự sai lầm thường mắc	Document	5 lầm tưởng dân gian có thể làm bệnh nhân chết nhanh hơn.	\N	10	2	2026-03-06 17:51:45.092952	\N
23	44	Bài 7: Hỗ trợ cấp cứu trước khi xe cứu thương đến	Document	Giữ thái độ bình tĩnh và điều phối người xung quanh.	\N	8	3	2026-03-06 17:51:45.092952	\N
24	45	Bài 8: Ôn tập toàn diện khóa học	Document	Tổng hợp checklist để xử lý Cấp cứu chấn thương sọ não cơ bản	\N	10	0	2026-03-06 17:51:45.092952	\N
25	45	Bài 9: Bài Thi Cấp Chứng Chỉ	Exam	Bạn có 30 phút để hoàn thành bài thi trắc nghiệm này để được công nhận.	\N	30	1	2026-03-06 17:51:45.092952	\N
46	52	Bài 3: Chuẩn bị thiết bị cần thiết	Document	Những thứ bạn cần có trong bộ sơ cứu chuyên dụng.	\N	10	2	2026-03-06 17:51:45.092952	\N
47	53	Bài 4: Phân tích đánh giá tình trạng bệnh nhân	Video	Sử dụng thang đo DRABC để đánh giá tổng quan.	https://www.youtube.com/watch?v=dQw4w9WgXcQ	20	0	2026-03-06 17:51:45.092952	\N
48	53	Bài 5: Hướng dẫn kỹ thuật thủ công	Document	Chi tiết các bước từ 1 đến 5 để thao tác.	\N	12	1	2026-03-06 17:51:45.092952	\N
49	53	Bài 6: Phân biệt sự sai lầm thường mắc	Document	5 lầm tưởng dân gian có thể làm bệnh nhân chết nhanh hơn.	\N	10	2	2026-03-06 17:51:45.092952	\N
50	53	Bài 7: Hỗ trợ cấp cứu trước khi xe cứu thương đến	Document	Giữ thái độ bình tĩnh và điều phối người xung quanh.	\N	8	3	2026-03-06 17:51:45.092952	\N
51	54	Bài 8: Ôn tập toàn diện khóa học	Document	Tổng hợp checklist để xử lý Huấn luyện Sơ cứu tại Công trường (OSHA chuẩn)	\N	10	0	2026-03-06 17:51:45.092952	\N
52	54	Bài 9: Bài Thi Cấp Chứng Chỉ	Exam	Bạn có 30 phút để hoàn thành bài thi trắc nghiệm này để được công nhận.	\N	30	1	2026-03-06 17:51:45.092952	\N
62	58	Bài 1: Tổng quan và Lời mở đầu	Document	Tình trạng Cấp cứu Hạ đường huyết & Biến chứng Tiểu đường là một trong những ca cấp cứu thường gặp nhất.	\N	5	0	2026-03-06 17:51:45.092952	\N
63	58	Bài 2: Tính khẩn cấp và Giờ vàng	Video	Khái niệm giờ vàng và tại sao bạn phải hành động trong phút đầu tiên.	https://www.youtube.com/watch?v=dQw4w9WgXcQ	15	1	2026-03-06 17:51:45.092952	\N
64	58	Bài 3: Chuẩn bị thiết bị cần thiết	Document	Những thứ bạn cần có trong bộ sơ cứu chuyên dụng.	\N	10	2	2026-03-06 17:51:45.092952	\N
65	59	Bài 4: Phân tích đánh giá tình trạng bệnh nhân	Video	Sử dụng thang đo DRABC để đánh giá tổng quan.	https://www.youtube.com/watch?v=dQw4w9WgXcQ	20	0	2026-03-06 17:51:45.092952	\N
66	59	Bài 5: Hướng dẫn kỹ thuật thủ công	Document	Chi tiết các bước từ 1 đến 5 để thao tác.	\N	12	1	2026-03-06 17:51:45.092952	\N
67	59	Bài 6: Phân biệt sự sai lầm thường mắc	Document	5 lầm tưởng dân gian có thể làm bệnh nhân chết nhanh hơn.	\N	10	2	2026-03-06 17:51:45.092952	\N
68	59	Bài 7: Hỗ trợ cấp cứu trước khi xe cứu thương đến	Document	Giữ thái độ bình tĩnh và điều phối người xung quanh.	\N	8	3	2026-03-06 17:51:45.092952	\N
69	60	Bài 8: Ôn tập toàn diện khóa học	Document	Tổng hợp checklist để xử lý Cấp cứu Hạ đường huyết & Biến chứng Tiểu đường	\N	10	0	2026-03-06 17:51:45.092952	\N
70	60	Bài 9: Bài Thi Cấp Chứng Chỉ	Exam	Bạn có 30 phút để hoàn thành bài thi trắc nghiệm này để được công nhận.	\N	30	1	2026-03-06 17:51:45.092952	\N
71	61	Bài 1: Tổng quan và Lời mở đầu	Document	Tình trạng Xử trí Say nắng, Cảm lạnh & Thay đổi Nhiệt độ Đột ngột là một trong những ca cấp cứu thường gặp nhất.	\N	5	0	2026-03-06 17:51:45.092952	\N
72	61	Bài 2: Tính khẩn cấp và Giờ vàng	Video	Khái niệm giờ vàng và tại sao bạn phải hành động trong phút đầu tiên.	https://www.youtube.com/watch?v=dQw4w9WgXcQ	15	1	2026-03-06 17:51:45.092952	\N
73	61	Bài 3: Chuẩn bị thiết bị cần thiết	Document	Những thứ bạn cần có trong bộ sơ cứu chuyên dụng.	\N	10	2	2026-03-06 17:51:45.092952	\N
74	62	Bài 4: Phân tích đánh giá tình trạng bệnh nhân	Video	Sử dụng thang đo DRABC để đánh giá tổng quan.	https://www.youtube.com/watch?v=dQw4w9WgXcQ	20	0	2026-03-06 17:51:45.092952	\N
75	62	Bài 5: Hướng dẫn kỹ thuật thủ công	Document	Chi tiết các bước từ 1 đến 5 để thao tác.	\N	12	1	2026-03-06 17:51:45.092952	\N
76	62	Bài 6: Phân biệt sự sai lầm thường mắc	Document	5 lầm tưởng dân gian có thể làm bệnh nhân chết nhanh hơn.	\N	10	2	2026-03-06 17:51:45.092952	\N
77	62	Bài 7: Hỗ trợ cấp cứu trước khi xe cứu thương đến	Document	Giữ thái độ bình tĩnh và điều phối người xung quanh.	\N	8	3	2026-03-06 17:51:45.092952	\N
78	63	Bài 8: Ôn tập toàn diện khóa học	Document	Tổng hợp checklist để xử lý Xử trí Say nắng, Cảm lạnh & Thay đổi Nhiệt độ Đột ngột	\N	10	0	2026-03-06 17:51:45.092952	\N
79	63	Bài 9: Bài Thi Cấp Chứng Chỉ	Exam	Bạn có 30 phút để hoàn thành bài thi trắc nghiệm này để được công nhận.	\N	30	1	2026-03-06 17:51:45.092952	\N
80	64	Bài 1: Tổng quan và Lời mở đầu	Document	Tình trạng Sơ cấp cứu Tâm lý (PFA) - Sốc tâm lý sau thảm họa là một trong những ca cấp cứu thường gặp nhất.	\N	5	0	2026-03-06 17:51:45.092952	\N
81	64	Bài 2: Tính khẩn cấp và Giờ vàng	Video	Khái niệm giờ vàng và tại sao bạn phải hành động trong phút đầu tiên.	https://www.youtube.com/watch?v=dQw4w9WgXcQ	15	1	2026-03-06 17:51:45.092952	\N
82	64	Bài 3: Chuẩn bị thiết bị cần thiết	Document	Những thứ bạn cần có trong bộ sơ cứu chuyên dụng.	\N	10	2	2026-03-06 17:51:45.092952	\N
83	65	Bài 4: Phân tích đánh giá tình trạng bệnh nhân	Video	Sử dụng thang đo DRABC để đánh giá tổng quan.	https://www.youtube.com/watch?v=dQw4w9WgXcQ	20	0	2026-03-06 17:51:45.092952	\N
84	65	Bài 5: Hướng dẫn kỹ thuật thủ công	Document	Chi tiết các bước từ 1 đến 5 để thao tác.	\N	12	1	2026-03-06 17:51:45.092952	\N
85	65	Bài 6: Phân biệt sự sai lầm thường mắc	Document	5 lầm tưởng dân gian có thể làm bệnh nhân chết nhanh hơn.	\N	10	2	2026-03-06 17:51:45.092952	\N
86	65	Bài 7: Hỗ trợ cấp cứu trước khi xe cứu thương đến	Document	Giữ thái độ bình tĩnh và điều phối người xung quanh.	\N	8	3	2026-03-06 17:51:45.092952	\N
87	66	Bài 8: Ôn tập toàn diện khóa học	Document	Tổng hợp checklist để xử lý Sơ cấp cứu Tâm lý (PFA) - Sốc tâm lý sau thảm họa	\N	10	0	2026-03-06 17:51:45.092952	\N
88	66	Bài 9: Bài Thi Cấp Chứng Chỉ	Exam	Bạn có 30 phút để hoàn thành bài thi trắc nghiệm này để được công nhận.	\N	30	1	2026-03-06 17:51:45.092952	\N
89	67	Bài 1: Tổng quan và Lời mở đầu	Document	Tình trạng Kỹ năng Di chuyển Nạn nhân an toàn ra khỏi Đám Cháy là một trong những ca cấp cứu thường gặp nhất.	\N	5	0	2026-03-06 17:51:45.092952	\N
90	67	Bài 2: Tính khẩn cấp và Giờ vàng	Video	Khái niệm giờ vàng và tại sao bạn phải hành động trong phút đầu tiên.	https://www.youtube.com/watch?v=dQw4w9WgXcQ	15	1	2026-03-06 17:51:45.092952	\N
91	67	Bài 3: Chuẩn bị thiết bị cần thiết	Document	Những thứ bạn cần có trong bộ sơ cứu chuyên dụng.	\N	10	2	2026-03-06 17:51:45.092952	\N
92	68	Bài 4: Phân tích đánh giá tình trạng bệnh nhân	Video	Sử dụng thang đo DRABC để đánh giá tổng quan.	https://www.youtube.com/watch?v=dQw4w9WgXcQ	20	0	2026-03-06 17:51:45.092952	\N
93	68	Bài 5: Hướng dẫn kỹ thuật thủ công	Document	Chi tiết các bước từ 1 đến 5 để thao tác.	\N	12	1	2026-03-06 17:51:45.092952	\N
94	68	Bài 6: Phân biệt sự sai lầm thường mắc	Document	5 lầm tưởng dân gian có thể làm bệnh nhân chết nhanh hơn.	\N	10	2	2026-03-06 17:51:45.092952	\N
95	68	Bài 7: Hỗ trợ cấp cứu trước khi xe cứu thương đến	Document	Giữ thái độ bình tĩnh và điều phối người xung quanh.	\N	8	3	2026-03-06 17:51:45.092952	\N
96	69	Bài 8: Ôn tập toàn diện khóa học	Document	Tổng hợp checklist để xử lý Kỹ năng Di chuyển Nạn nhân an toàn ra khỏi Đám Cháy	\N	10	0	2026-03-06 17:51:45.092952	\N
97	69	Bài 9: Bài Thi Cấp Chứng Chỉ	Exam	Bạn có 30 phút để hoàn thành bài thi trắc nghiệm này để được công nhận.	\N	30	1	2026-03-06 17:51:45.092952	\N
98	70	Bài 1: Tổng quan và Lời mở đầu	Document	Tình trạng Phòng ngừa và Cấp cứu Ngạt khí độc (CO, Metan) là một trong những ca cấp cứu thường gặp nhất.	\N	5	0	2026-03-06 17:51:45.092952	\N
99	70	Bài 2: Tính khẩn cấp và Giờ vàng	Video	Khái niệm giờ vàng và tại sao bạn phải hành động trong phút đầu tiên.	https://www.youtube.com/watch?v=dQw4w9WgXcQ	15	1	2026-03-06 17:51:45.092952	\N
100	70	Bài 3: Chuẩn bị thiết bị cần thiết	Document	Những thứ bạn cần có trong bộ sơ cứu chuyên dụng.	\N	10	2	2026-03-06 17:51:45.092952	\N
101	71	Bài 4: Phân tích đánh giá tình trạng bệnh nhân	Video	Sử dụng thang đo DRABC để đánh giá tổng quan.	https://www.youtube.com/watch?v=dQw4w9WgXcQ	20	0	2026-03-06 17:51:45.092952	\N
102	71	Bài 5: Hướng dẫn kỹ thuật thủ công	Document	Chi tiết các bước từ 1 đến 5 để thao tác.	\N	12	1	2026-03-06 17:51:45.092952	\N
103	71	Bài 6: Phân biệt sự sai lầm thường mắc	Document	5 lầm tưởng dân gian có thể làm bệnh nhân chết nhanh hơn.	\N	10	2	2026-03-06 17:51:45.092952	\N
104	71	Bài 7: Hỗ trợ cấp cứu trước khi xe cứu thương đến	Document	Giữ thái độ bình tĩnh và điều phối người xung quanh.	\N	8	3	2026-03-06 17:51:45.092952	\N
105	72	Bài 8: Ôn tập toàn diện khóa học	Document	Tổng hợp checklist để xử lý Phòng ngừa và Cấp cứu Ngạt khí độc (CO, Metan)	\N	10	0	2026-03-06 17:51:45.092952	\N
106	72	Bài 9: Bài Thi Cấp Chứng Chỉ	Exam	Bạn có 30 phút để hoàn thành bài thi trắc nghiệm này để được công nhận.	\N	30	1	2026-03-06 17:51:45.092952	\N
111	76	Cách sơ cứu nhồi máu cơ tim	Video	5 dấu hiệu chính của đột quỵ:\r\n\r\nMéo mặt – một bên mặt bị xệ, cười không đều.\r\n\r\nYếu hoặc tê tay chân – thường ở một bên cơ thể.\r\n\r\nNói khó hoặc nói ngọng – khó phát âm hoặc không hiểu lời nói.\r\n\r\nChóng mặt, mất thăng bằng – đi đứng loạng choạng.\r\n\r\nĐau đầu dữ dội đột ngột – khác hẳn những cơn đau đầu bình thường.	https://youtu.be/xONSnLw-pRI	10	1	2026-03-11 09:29:26.88166	Sau khi hoàn thành user có thể nhận biết và xử lý khi có người cần sơ cứu khi bị đột quỵ nhồi máu cơ tim
\.


--
-- Data for Name: course_objectives; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.course_objectives (id, course_id, content) FROM stdin;
36	16	Hiểu rõ nguyên tắc cơ bản trong cấp cứu ban đầu.
37	16	Thực hiện chính xác kỹ thuật ép tim ngoài lồng ngực (CPR) cho mọi lứa tuổi.
38	16	Biết cách vận hành máy sốc điện tự động (AED).
39	16	Xử lý hiệu quả hóc dị vật đường thở bằng thủ thuật Heimlich.
40	16	Thực hành băng bó, cầm máu, cố định xương gãy cơ bản.
50	22	Nhận diện chấn thương sọ não
51	22	Cố định cột sống cổ
52	22	Cách di chuyển an toàn
59	25	Ngắt nguồn điện an toàn
60	25	Sơ cứu điện tim giật
61	25	Cách gỡ người bị kẹt máy móc
65	27	Nhận diện hạ đường huyết
66	27	Cách pha dung dịch đường cấp cứu
67	27	Kỹ thuật đặt bệnh nhân nằm nghiêng
68	28	Chườm mát khoa học
69	28	Nhận diện say nắng mức độ nặng
70	28	Tuyệt đối không nhúng bệnh nhân vào nước đá
71	29	Quy tắc Lắng nghe Tâm lý
72	29	Tránh đặt câu hỏi dồn dập
73	29	Tạo vùng không gian an toàn cho người sốc
74	30	Tư thế tránh khói độc
75	30	Cách khiêng người bất tỉnh chuẩn
76	30	Chặn khói tràn vào phòng
77	31	Nhận diện mùi khí dễ cháy nổ
78	31	Thông gió an toàn không dùng điện
79	31	Hô hấp nhân tạo cho người ngạt khí
82	36	Xử lý sơ cứu khẩn cấp trong trường hợp có người cần sơ cấp cứu đột quỵ
\.


--
-- Data for Name: course_syllabus; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.course_syllabus (id, course_id, title, duration, lesson_count) FROM stdin;
38	16	Chương 2: CPR Chuyên Sâu & Sử Dụng Máy Sốc Điện (AED)	4 giờ	4
39	16	Chương 3: Cầm Máu, Bỏng & Bài Thi Thực Tế Hoàn Thành Khóa	6 giờ	4
37	16	Chương 1: Nguyên Tắc Khẩn Cấp & Đánh Giá Nạn Nhân	2 giờ	4
43	22	Chương 1: Các khái niệm cơ bản cần nắm	\N	3
44	22	Chương 2: Xử trí bước một và Hai	\N	4
45	22	Chương 3: Ôn tập và Thi chứng chỉ	\N	2
52	25	Chương 1: Các khái niệm cơ bản cần nắm	\N	3
53	25	Chương 2: Xử trí bước một và Hai	\N	4
54	25	Chương 3: Ôn tập và Thi chứng chỉ	\N	2
58	27	Chương 1: Các khái niệm cơ bản cần nắm	\N	3
59	27	Chương 2: Xử trí bước một và Hai	\N	4
60	27	Chương 3: Ôn tập và Thi chứng chỉ	\N	2
61	28	Chương 1: Các khái niệm cơ bản cần nắm	\N	3
62	28	Chương 2: Xử trí bước một và Hai	\N	4
63	28	Chương 3: Ôn tập và Thi chứng chỉ	\N	2
64	29	Chương 1: Các khái niệm cơ bản cần nắm	\N	3
65	29	Chương 2: Xử trí bước một và Hai	\N	4
66	29	Chương 3: Ôn tập và Thi chứng chỉ	\N	2
67	30	Chương 1: Các khái niệm cơ bản cần nắm	\N	3
68	30	Chương 2: Xử trí bước một và Hai	\N	4
69	30	Chương 3: Ôn tập và Thi chứng chỉ	\N	2
70	31	Chương 1: Các khái niệm cơ bản cần nắm	\N	3
71	31	Chương 2: Xử trí bước một và Hai	\N	4
72	31	Chương 3: Ôn tập và Thi chứng chỉ	\N	2
76	36	Nhận biết và cách sơ cấp cứu đúng cho người bị đột quỵ nhồi máu cơ tim	0	1
\.


--
-- Data for Name: courses; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.courses (id, title, description, certificate_name, image_url, is_popular, created_at, video_url, training_image_url, instructor_id, is_active, category) FROM stdin;
16	Cấp Cứu Ban Đầu & CPR Toàn Diện (Chứng Chỉ Trọn Đời)	Khóa học cung cấp kiến thức và kỹ năng thực hành đầy đủ nhất về sơ cấp cứu ban đầu cho mọi độ tuổi (người lớn, trẻ em, trẻ sơ sinh). Bao gồm CPR, sử dụng máy AED, xử lý nghẹt thở, cầm máu, và các chấn thương thường gặp. Khóa học có các bài tập tình huống thực tế giúp học viên tự tin ứng phó trong các trường hợp khẩn cấp.	Chứng Chỉ Sơ Cấp Cứu Quốc Gia	/images/courses/a774c115-21c8-4ddd-82a4-88cb1825ea53_2.jpg	t	2026-03-06 12:22:36.840805	https://www.youtube.com/watch?v=Cgimy5covI4&list=RDPKWwDoNtCKE&index=2	\N	5	t	Chung
22	Cấp cứu chấn thương sọ não cơ bản	Tìm hiểu cách nhận biết các dấu hiệu tổn thương não bộ, cách giữ cố định cột sống cổ và những việc tuyệt đối không được làm khi có nghi ngờ chấn thương sọ não.	\N	https://images.unsplash.com/photo-1584036561566-baf8f5f1b144?w=800	f	2026-03-06 17:51:45.092952	https://www.youtube.com/watch?v=dQw4w9WgXcQ	\N	5	t	Cấp cứu
25	Huấn luyện Sơ cứu tại Công trường (OSHA chuẩn)	Khóa học chuyên sâu dành riêng cho công nhân, kỹ sư giám sát tại các công trường xây dựng, bao gồm xử lý điện giật, ngã giàn giáo và tai nạn máy móc.	\N	https://images.unsplash.com/photo-1584036561566-baf8f5f1b144?w=800	t	2026-03-06 17:51:45.092952	https://www.youtube.com/watch?v=dQw4w9WgXcQ	\N	5	t	Huấn luyện
27	Cấp cứu Hạ đường huyết & Biến chứng Tiểu đường	Kiến thức cực kỳ quan trọng cho người nhà bệnh nhân tiểu đường: Phân biệt hôn mê do hạ đường hoặc tăng đường huyết và cách cấp cứu bằng đường mạch nha/nước ngọt.	\N	https://images.unsplash.com/photo-1584036561566-baf8f5f1b144?w=800	f	2026-03-06 17:51:45.092952	https://www.youtube.com/watch?v=dQw4w9WgXcQ	\N	5	t	Cấp cứu
28	Xử trí Say nắng, Cảm lạnh & Thay đổi Nhiệt độ Đột ngột	Nhận biết triệu chứng rối loạn thân nhiệt do thời tiết khắc nghiệt: Sốc nhiệt mùa hè và Hạ thân nhiệt mùa đông. Cách bù nước, điện giải và làm sơ cứu.	\N	https://images.unsplash.com/photo-1584036561566-baf8f5f1b144?w=800	f	2026-03-06 17:51:45.092952	https://www.youtube.com/watch?v=dQw4w9WgXcQ	\N	5	t	Xử trí
29	Sơ cấp cứu Tâm lý (PFA) - Sốc tâm lý sau thảm họa	Sơ cứu không chỉ cho thể xác. Khóa học này dạy bạn cách tiếp cận, an ủi, hỗ trợ tinh thần cho các nạn nhân vừa trải qua cú sốc tâm lý mạnh (tai nạn, hỏa hoạn, mất mát).	\N	https://images.unsplash.com/photo-1584036561566-baf8f5f1b144?w=800	t	2026-03-06 17:51:45.092952	https://www.youtube.com/watch?v=dQw4w9WgXcQ	\N	5	t	Chung
31	Phòng ngừa và Cấp cứu Ngạt khí độc (CO, Metan)	Phân tích cơ chế gây ngạt của các khí độc sinh hoạt (khí CO từ máy phát điện trong nhà kín, khí Metan hầm cầu) và giải pháp đưa bệnh nhân ra môi trường an toàn.	\N	https://images.unsplash.com/photo-1584036561566-baf8f5f1b144?w=800	f	2026-03-06 17:51:45.092952	https://www.youtube.com/watch?v=dQw4w9WgXcQ	\N	5	t	Cấp cứu
30	Kỹ năng Di chuyển Nạn nhân an toàn ra khỏi Đám Cháy	Kỹ năng sống còn trong hỏa hoạn: Cách tạo mặt nạ lọc khói tạm thời, tư thế bò dưới khói hạn chế ngạt thở, và kỹ thuật kéo lê nạn nhân bất tỉnh ra khỏi vùng nguy hiểm.	\N	https://images.unsplash.com/photo-1584036561566-baf8f5f1b144?w=800	f	2026-03-06 17:51:45.092952	https://www.youtube.com/watch?v=dQw4w9WgXcQ	\N	5	t	Kỹ năng
36	Đột quỵ nhồi máu cơ tim 3	5 dấu hiệu chính của đột quỵ:\r\n\r\nMéo mặt – một bên mặt bị xệ, cười không đều.\r\n\r\nYếu hoặc tê tay chân – thường ở một bên cơ thể.\r\n\r\nNói khó hoặc nói ngọng – khó phát âm hoặc không hiểu lời nói.\r\n\r\nChóng mặt, mất thăng bằng – đi đứng loạng choạng.\r\n\r\nĐau đầu dữ dội đột ngột – khác hẳn những cơn đau đầu bình thường.	\N	/images/courses/84f88c1e-83df-48a2-83f9-2771787e8891_Độtquy.jpg	t	2026-03-11 16:27:13.641989	https://youtu.be/xONSnLw-pRI	\N	5	t	Y tế & Sức khỏe
\.


--
-- Data for Name: enrollments; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.enrollments (id, user_id, course_id, enrolled_at, status, amount) FROM stdin;
4	18	16	2026-03-06 05:22:50.622463	Active	0.00
8	3	16	2026-03-11 01:28:12.223651	Active	0.00
10	3	31	2026-03-11 01:41:10.456073	Active	0.00
13	18	29	2026-03-11 03:40:15.972732	Active	0.00
\.


--
-- Data for Name: family_course_categories; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.family_course_categories (id, title, description, icon_url, color_hex) FROM stdin;
1	An toàn ở nhà	Các tình huống nguy hiểm thường gặp trong môi trường gia đình.	fas fa-home	#4caf50
2	Vui chơi ngoài trời	Xử lý các sự cố khi đi dã ngoại, công viên, hồ bơi.	fas fa-tree	#ff9800
3	Cấp cứu cơ bản	Nhận biết và sơ cứu các chấn thương phổ biến.	fas fa-first-aid	#f44336
4	Sơ cứu hóc dị vật ở trẻ em	Tình huống khấn cấp khi trẻ bị hóc dị vật (thức ăn, đồ chơi nhỏ). Học cách phản ứng nhanh chóng và chính xác để cứu sống trẻ.	https://cdn-icons-png.flaticon.com/512/3233/3233483.png	#e74c3c
5	Sơ cứu hóc dị vật ở trẻ em	Tình huống khấn cấp khi trẻ bị hóc dị vật (thức ăn, đồ chơi nhỏ). Học cách phản ứng nhanh chóng và chính xác để cứu sống trẻ.	https://cdn-icons-png.flaticon.com/512/3233/3233483.png	#e74c3c
\.


--
-- Data for Name: feedbacks; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.feedbacks (id, user_id, course_id, rating, comment, created_at) FROM stdin;
3	18	16	5	 Rất đỉnh	2026-03-10 03:48:12.046437+07
20	3	16	3	Web còn cần nhiều cải thiện về trải nghiệm người dùng	2026-03-18 04:23:06.021745+07
\.


--
-- Data for Name: game_category_experts; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.game_category_experts (id, expert_id, category_id, assigned_at) FROM stdin;
9	25	1	2026-03-10 13:15:44.138173
10	25	2	2026-03-10 13:15:44.138245
11	25	3	2026-03-10 13:15:44.138268
12	25	4	2026-03-10 13:15:44.138282
13	25	5	2026-03-10 13:15:44.138304
\.


--
-- Data for Name: game_options; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.game_options (id, situation_id, option_text, is_correct, explanation, points, icon_url) FROM stdin;
1	1	Bôi kem đánh răng lên vết bỏng ngay lập tức.	f	Sai lầm! Kem đánh răng có tính kiềm, có thể làm vết bỏng sâu hơn và dễ nhiễm trùng.	0	fas fa-times-circle
2	2	Xả vết bỏng dưới vòi nước mát chảy nhẹ khoảng 15-20 phút.	t	Chính xác! Nước mát giúp hạ nhiệt vùng da bị bỏng, giảm sưng đau và ngăn tổn thương sâu.	100	fas fa-tint
3	3	Lấy đá lạnh chườm trực tiếp lên vết bỏng.	f	Tuyệt đối không! Đá lạnh có thể gây "bỏng lạnh" khiến vùng da bị tổn thương nặng nề hơn.	0	fas fa-snowflake
4	4	Băng kín vết bỏng lại bằng băng dính.	f	Không nên băng kín ngay, điều này làm nhiệt không thể thoát ra và bong tróc da khi tháo băng.	0	fas fa-band-aid
5	2	Cho bé uống một ngụm nước lớn để vật trôi xuống.	f	Rất nguy hiểm! Nước có thể làm dị vật lọt sâu hơn vào đường thở gây ngạt thở hoàn toàn.	0	fas fa-glass-water
6	2	Lấy tay chọc thẳng vào họng bé để móc dị vật ra.	f	Không được móc họng mù! Việc này có thể đẩy dị vật vào sâu hơn.	0	fas fa-hand-paper
7	2	Thực hiện vỗ lưng 5 lần và ấn ngực 5 lần (Nghiệm pháp Heimlich cho trẻ nhỏ).	t	Chính xác! Đây là kỹ thuật chuẩn để tạo áp lực đẩy dị vật ra khỏi đường thở của trẻ.	100	fas fa-hands-helping
8	2	Bảo bé tự ho thật mạnh.	f	Bé 2 tuổi đang tím tái và không khóc được (nghĩa là nghẹt đường thở hoàn toàn), bé không thể tự ho được nữa.	0	fas fa-cough
9	3	Nặn bóp mạnh vết cắn để nặn máu và nọc độc ra ngoài.	f	Việc nặn bóp sẽ làm nọc độc phát tán nhanh hơn vào máu.	0	fas fa-hand-holding-water
10	3	Dùng thẻ ATM (hoặc vật có cạnh cứng) gạt nhẹ để lấy ngòi ong ra, sau đó rửa xà phòng.	t	Rất tốt! Gạt ngang thay vì nhổ sẽ tránh việc bóp thêm nọc độc vào da. Rửa xà phòng giúp sát khuẩn.	100	fas fa-credit-card
11	3	Bôi nước mắm hoặc kem đánh răng lên vết đốt.	f	Đây là mẹo dân gian không có cơ sở khoa học, dễ gây nhiễm trùng vết thương.	0	fas fa-vial
12	4	Ngửa đầu ra phía sau thật xa để máu chảy ngược vào trong.	f	Tuyệt đối không! Máu chảy ngược vào họng có thể tràn vào phổi hoặc dạ dày gây buồn nôn.	0	fas fa-arrow-up
13	4	Nhét giấy ăn vào sâu trong mũi để nút lại.	f	Giấy ăn không sạch 100%, có thể gây nhiễm trùng. Hơn nữa, khi rút ra có thể làm cục máu đông vỡ lại.	0	fas fa-toilet-paper
14	4	Ngồi cúi đầu nhẹ về phía trước, dùng tay bóp chặt bích mũi khoảng 5-10 phút, thở bằng miệng.	t	Chính xác! Cúi ra trước để máu chảy ra ngoài, bóp chặt để tạo áp lực cầm máu.	100	fas fa-arrow-down
15	5	Cố gắng thò ngón tay vào miệng bé để móc dị vật ra.	f	Tuyệt đối KHÔNG dùng ngón tay móc mù (khi không nhìn thấy dị vật). Việc này có thể đẩy dị vật vào sâu hơn, gây tắc nghẽn hoàn toàn đường thở.	-10	\N
16	5	Khuyến khích bé ho và theo dõi sát sao.	t	Nếu bé vẫn đang ho và khóc được nghĩa là đường thở chỉ tắc nghẽn một phần. Phản xạ ho là cách tốt nhất để đẩy dị vật ra ngoài.	20	\N
17	5	Dốc ngược bé lên lập tức và vỗ lưng.	f	Trẻ đang ho và khóc được không cần can thiệp vật lý như vỗ lưng ngay. Can thiệp lúc này có thể làm dị vật rơi sâu hơn.	-5	\N
18	5	Cho bé uống một ngụm nước lớn để trôi dị vật.	f	Không bao giờ cho uống nước khi đang hóc, nước có thể tràn vào phổi gây sặc hoặc làm dị vật (nếu là dạng bánh/kẹo) nở ra gây tắc thêm.	-10	\N
19	6	Thực hiện thủ thuật Heimlich (vỗ lưng, ấn ngực/bụng) ngay lập tức.	t	Khẩn cấp! Đây là dấu hiệu tắc nghẽn đường thở hoàn toàn. Phải lập tức thực hiện 5 lần vỗ lưng, luân phiên 5 lần ép ngực/bụng tùy độ tuổi.	30	\N
20	6	Gọi cấp cứu 115 và ngồi chờ bác sĩ tới.	f	Gọi cấp cứu là đúng, NHƯNG ngồi chờ sẽ làm trẻ tử vong do ngạt thở. Phải tiến hành sơ cứu ngay trong lúc nhờ người khác gọi 115.	-20	\N
21	6	Vuốt ngực bé từ trên xuống dưới cho xuôi.	f	Hành động vuốt ngực không có tác dụng đẩy dị vật ra khỏi đường thở, làm lãng phí thời gian vàng để cứu trẻ.	-10	\N
22	7	Bế bé ngồi thẳng trên đùi bạn, úp ngực bé vào ngực bạn.	f	Tư thế này không sử dụng được trọng lực để giúp dị vật rơi ra ngoài.	-5	\N
23	7	Để bé nằm ngửa trên giường cứng.	f	Nằm ngửa là tư thế để ép ngực (sau khi vỗ lưng không thành công), không phải tư thế vỗ lưng.	-5	\N
24	7	Đặt bé nằm sấp dọc theo cánh tay bạn, đầu thấp hơn ngực, đỡ vằm và cổ bé.	t	Đúng! Tư thế đầu thấp hơn ngực giúp trọng lực hỗ trợ đẩy dị vật ra. Phải đỡ chắc phần cằm/cổ bé nhưng không bóp vào cổ họng.	25	\N
25	8	Cố gắng thò ngón tay vào miệng bé để móc dị vật ra.	f	Tuyệt đối KHÔNG dùng ngón tay móc mù (khi không nhìn thấy dị vật). Việc này có thể đẩy dị vật vào sâu hơn, gây tắc nghẽn hoàn toàn đường thở.	-10	\N
26	8	Khuyến khích bé ho và theo dõi sát sao.	t	Nếu bé vẫn đang ho và khóc được nghĩa là đường thở chỉ tắc nghẽn một phần. Phản xạ ho là cách tốt nhất để đẩy dị vật ra ngoài.	20	\N
27	8	Dốc ngược bé lên lập tức và vỗ lưng.	f	Trẻ đang ho và khóc được không cần can thiệp vật lý như vỗ lưng ngay. Can thiệp lúc này có thể làm dị vật rơi sâu hơn.	-5	\N
28	8	Cho bé uống một ngụm nước lớn để trôi dị vật.	f	Không bao giờ cho uống nước khi đang hóc, nước có thể tràn vào phổi gây sặc hoặc làm dị vật (nếu là dạng bánh/kẹo) nở ra gây tắc thêm.	-10	\N
29	9	Thực hiện thủ thuật Heimlich (vỗ lưng, ấn ngực/bụng) ngay lập tức.	t	Khẩn cấp! Đây là dấu hiệu tắc nghẽn đường thở hoàn toàn. Phải lập tức thực hiện 5 lần vỗ lưng, luân phiên 5 lần ép ngực/bụng tùy độ tuổi.	30	\N
30	9	Gọi cấp cứu 115 và ngồi chờ bác sĩ tới.	f	Gọi cấp cứu là đúng, NHƯNG ngồi chờ sẽ làm trẻ tử vong do ngạt thở. Phải tiến hành sơ cứu ngay trong lúc nhờ người khác gọi 115.	-20	\N
31	9	Vuốt ngực bé từ trên xuống dưới cho xuôi.	f	Hành động vuốt ngực không có tác dụng đẩy dị vật ra khỏi đường thở, làm lãng phí thời gian vàng để cứu trẻ.	-10	\N
32	10	Bế bé ngồi thẳng trên đùi bạn, úp ngực bé vào ngực bạn.	f	Tư thế này không sử dụng được trọng lực để giúp dị vật rơi ra ngoài.	-5	\N
33	10	Để bé nằm ngửa trên giường cứng.	f	Nằm ngửa là tư thế để ép ngực (sau khi vỗ lưng không thành công), không phải tư thế vỗ lưng.	-5	\N
34	10	Đặt bé nằm sấp dọc theo cánh tay bạn, đầu thấp hơn ngực, đỡ vằm và cổ bé.	t	Đúng! Tư thế đầu thấp hơn ngực giúp trọng lực hỗ trợ đẩy dị vật ra. Phải đỡ chắc phần cằm/cổ bé nhưng không bóp vào cổ họng.	25	\N
\.


--
-- Data for Name: game_situations; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.game_situations (id, category_id, title, character_context, situation_description, question, image_url) FROM stdin;
1	1	Phỏng nước sôi	Bé Bi (4 tuổi) đang chơi trong bếp.	Mẹ vừa rót 1 ly nước sôi để trên bàn. Bé Bi với tay lấy đồ chơi và vô tình làm đổ ly nước lên tay.	Bạn sẽ làm gì ĐẦU TIÊN để sơ cứu cho bé Bi?	https://cdn-icons-png.flaticon.com/512/3209/3209935.png
2	1	Hóc dị vật	Bé Na (2 tuổi) đang ngồi chơi đồ chơi xếp hình.	Đột nhiên bé Na ho sặc sụa, mặt tím tái và chỉ tay vào cổ họng. Bé không thể khóc thành tiếng.	Dấu hiệu này cho thấy bé Na bị làm sao và bạn cần làm gì?	https://cdn-icons-png.flaticon.com/512/2854/2854346.png
3	2	Ong đốt	Hai anh em Tèo và Tí đang chơi đá bóng ngoài công viên.	Tèo vô tình đá quả bóng vào một bụi cây và bị một con ong cắn vào bắp tay. Vết cắn bắt đầu sưng đỏ và đau nhức.	Cách xử lý nào dưới đây là ĐÚNG NHẤT?	https://cdn-icons-png.flaticon.com/512/2550/2550419.png
4	3	Chảy máu cam	Bé Bo (7 tuổi) đang ngồi xem TV.	Trời hanh khô, tự nhiên mũi bé Bo chảy máu ròng ròng xuống áo áo.	Bạn nên bảo bé Bo quay đầu như thế nào để cầm máu?	https://cdn-icons-png.flaticon.com/512/5770/5770617.png
5	4	Bé đang ăn dặm bỗng ho sặc sụa	Bạn đang cho bé (10 tháng tuổi) ăn dặm bằng trái cây cắt nhỏ.	Bé đột nhiên ho sặc sụa, mặt đỏ bừng lên. Bé vẫn có thể ho và khóc phát ra tiếng nhỏ.	Bạn nên làm gì đầu tiên?	\N
6	4	Trẻ ôm cổ, không ho, không khóc được	Bé (2 tuổi) đang chơi đồ chơi xếp hình bằng nhựa nhỏ.	Đột nhiên bé buông đồ chơi, hai tay ôm lấy cổ (dấu hiệu hóc dị vật), miệng há hốc nhưng không phát ra tiếng động nào. Môi bé bắt đầu tím tái.	Tình trạng của bé nghiêm trọng. Hành động ngay lập tức của bạn là gì?	\N
7	4	Thủ thuật vỗ lưng cho trẻ dưới 1 tuổi	Bạn xác định cần thực hiện vỗ lưng cho bé sơ sinh (dưới 1 tuổi) do bé nghẹt thở hoàn toàn.	Bạn bế bé lên để chuẩn bị thao tác vỗ lưng.	Tư thế ĐÚNG để vỗ lưng cho trẻ dưới 1 tuổi là gì?	\N
8	5	Bé đang ăn dặm bỗng ho sặc sụa	Bạn đang cho bé (10 tháng tuổi) ăn dặm bằng trái cây cắt nhỏ.	Bé đột nhiên ho sặc sụa, mặt đỏ bừng lên. Bé vẫn có thể ho và khóc phát ra tiếng nhỏ.	Bạn nên làm gì đầu tiên?	\N
9	5	Trẻ ôm cổ, không ho, không khóc được	Bé (2 tuổi) đang chơi đồ chơi xếp hình bằng nhựa nhỏ.	Đột nhiên bé buông đồ chơi, hai tay ôm lấy cổ (dấu hiệu hóc dị vật), miệng há hốc nhưng không phát ra tiếng động nào. Môi bé bắt đầu tím tái.	Tình trạng của bé nghiêm trọng. Hành động ngay lập tức của bạn là gì?	\N
10	5	Thủ thuật vỗ lưng cho trẻ dưới 1 tuổi	Bạn xác định cần thực hiện vỗ lưng cho bé sơ sinh (dưới 1 tuổi) do bé nghẹt thở hoàn toàn.	Bạn bế bé lên để chuẩn bị thao tác vỗ lưng.	Tư thế ĐÚNG để vỗ lưng cho trẻ dưới 1 tuổi là gì?	\N
\.


--
-- Data for Name: lesson_comments; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.lesson_comments (id, lesson_id, user_id, content, created_at, parent_id) FROM stdin;
2	2	18	phần đó làm như nào vậy à ?	2026-03-06 12:28:41.249635	\N
3	2	5	à cái đó thì bạn cứ trỏ vào là xong	2026-03-06 12:29:00.263938	2
4	2	5	123	2026-03-10 04:23:36.060272	\N
5	2	5	3	2026-03-10 04:23:41.414725	\N
6	2	5	123123	2026-03-10 04:23:44.691368	\N
7	2	5	3	2026-03-10 04:41:33.045161	\N
8	2	18	3	2026-03-10 04:41:44.809082	\N
9	3	18	bài này nếu đúng thì là như nào 	2026-03-10 05:14:05.299291	\N
12	2	18	123	2026-03-11 07:24:12.524695	2
13	3	18	123	2026-03-11 07:31:25.490118	\N
14	3	18	123	2026-03-14 05:03:43.362384	9
15	3	18	@Nguyễn Hữu Huy 123	2026-03-14 05:03:45.712829	9
16	1	18	bài này hay thật	2026-03-14 05:05:30.326723	\N
\.


--
-- Data for Name: lesson_notes; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.lesson_notes (id, lesson_id, user_id, content, video_timestamp, created_at, updated_at) FROM stdin;
1	1	18	123	\N	2026-03-14 05:03:19.152329+07	\N
2	1	18	123	\N	2026-03-14 05:05:20.185002+07	\N
3	3	18	123123	\N	2026-04-03 07:59:43.937008+07	\N
\.


--
-- Data for Name: medical_records; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.medical_records (id, user_id, condition_name, description, year_diagnosed, created_at) FROM stdin;
1	5	Huyết áp cao	123	2024	2026-02-25 12:34:17.051756+07
2	5	huyết áp cao	Stories track functionality or features expressed as user goals.	2024	2026-02-25 13:11:32.017553+07
5	3	Cao Huyết Áp	thiazide	2026	2026-03-06 22:17:20.21043+07
\.


--
-- Data for Name: messages; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.messages (id, sender_id, receiver_id, content, created_at) FROM stdin;
2	18	5	xin chào	2026-03-07 10:34:36.548318
3	5	18	chào bạn	2026-03-07 10:34:43.935885
4	18	5	tôi có thể hỏi bạn về vấn đề khóa học trên được không	2026-03-07 10:35:38.10135
5	5	18	ok	2026-03-07 10:46:16.144269
6	18	5	123	2026-03-07 10:47:09.99506
7	18	5	123	2026-03-07 10:50:10.558757
8	18	5	ok bạn	2026-03-07 10:50:23.944442
9	5	18	123	2026-03-07 10:54:50.444261
10	18	5	123	2026-03-07 10:54:57.782525
11	18	5	123	2026-03-07 10:54:59.622514
12	18	5	123	2026-03-07 10:55:01.055641
14	18	5	xin chào 	2026-03-12 14:39:35.334284
15	18	5	cho mình hỏi là khóa này học trong bao lâu vậy	2026-03-12 14:47:32.194552
\.


--
-- Data for Name: notifications; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.notifications (id, user_id, title, message, link, is_read, created_at) FROM stdin;
1	5	Học viên mới	Nguyễn Hữu Huy vừa đăng ký khóa học: Xử trí cầm máu băng gạc từ vật dụng hàng ngày	/Expert/MyCourses	t	2026-03-06 11:07:20.807878
2	5	Câu hỏi mới	Nguyễn Hữu Huy đã đặt câu hỏi trong bài: Bài 1: Tổng quan và Lời mở đầu	/Course/Lesson/26#comments	t	2026-03-06 11:16:33.850242
4	18	Câu hỏi của bạn đã được trả lời	Chuyên gia có chứng chỉ đã trả lời câu hỏi của bạn trong bài: Bài 2: Đánh giá nhanh tình trạng nạn nhân (DRABC)	/Course/Lesson/2#qa	t	2026-03-06 12:29:00.264567
3	5	Câu hỏi mới	Nguyễn Hữu Huy đã đặt câu hỏi trong bài: Bài 2: Đánh giá nhanh tình trạng nạn nhân (DRABC)	/Course/Lesson/2#comments	t	2026-03-06 12:28:41.261077
5	5	Học viên mới	Nguyễn Văn A vừa đăng ký khóa học: Đột Quỵ	/Expert/MyCourses	t	2026-03-06 15:26:20.998446
6	5	Câu hỏi mới	Chuyên gia có chứng chỉ đã đặt câu hỏi trong bài: Bài 2: Đánh giá nhanh tình trạng nạn nhân (DRABC)	/Course/Lesson/2#comments	f	2026-03-10 04:23:36.062986
7	5	Câu hỏi mới	Chuyên gia có chứng chỉ đã đặt câu hỏi trong bài: Bài 2: Đánh giá nhanh tình trạng nạn nhân (DRABC)	/Course/Lesson/2#comments	f	2026-03-10 04:23:41.41535
8	5	Câu hỏi mới	Chuyên gia có chứng chỉ đã đặt câu hỏi trong bài: Bài 2: Đánh giá nhanh tình trạng nạn nhân (DRABC)	/Course/Lesson/2#comments	f	2026-03-10 04:23:44.691585
9	5	Câu hỏi mới	Chuyên gia có chứng chỉ đã đặt câu hỏi trong bài: Bài 2: Đánh giá nhanh tình trạng nạn nhân (DRABC)	/Course/Lesson/2#comments	f	2026-03-10 04:41:33.056359
10	5	Câu hỏi mới	Nguyễn Hữu Huy đã đặt câu hỏi trong bài: Bài 2: Đánh giá nhanh tình trạng nạn nhân (DRABC)	/Course/Lesson/2#comments	f	2026-03-10 04:41:44.809691
11	5	Câu hỏi mới	Nguyễn Hữu Huy đã đặt câu hỏi trong bài: Bài 3: Bài tập tình huống thực tế số 1	/Course/Lesson/3#comments	f	2026-03-10 05:14:05.310275
12	25	Chứng chỉ đã được duyệt	Chứng chỉ "Chứng chỉ chuyên môn" của bạn đã được quản trị viên phê duyệt.	/Expert/Qualifications	f	2026-03-10 10:24:01.353877
14	5	Học viên mới	Nguyễn Văn A vừa đăng ký khóa học: Cấp Cứu Ban Đầu & CPR Toàn Diện (Chứng Chỉ Trọn Đời)	/Expert/Dashboard	f	2026-03-11 01:28:12.305851
15	5	Học viên mới	Nguyễn Văn A vừa đăng ký khóa học: Đột quỵ , nhồi máu cơ tim	/Expert/Dashboard	f	2026-03-11 01:40:06.052625
16	5	Học viên mới	Nguyễn Văn A vừa đăng ký khóa học: Phòng ngừa và Cấp cứu Ngạt khí độc (CO, Metan)	/Expert/Dashboard	f	2026-03-11 01:41:10.46046
17	5	Học viên mới	Nguyễn Văn A vừa đăng ký khóa học: Đột quỵ mãng tính	/Expert/Dashboard	f	2026-03-11 01:41:30.510763
18	5	Học viên mới	Nguyễn Hữu Huy vừa đăng ký khóa học: Đột quỵ mãng tính	/Expert/Dashboard	f	2026-03-11 03:39:24.893128
19	5	Học viên mới	Nguyễn Hữu Huy vừa đăng ký khóa học: Sơ cấp cứu Tâm lý (PFA) - Sốc tâm lý sau thảm họa	/Expert/Dashboard	f	2026-03-11 03:40:15.973916
20	5	Học viên mới	Nguyễn Văn A vừa đăng ký khóa học: Tăng huyết áp 	/Expert/Dashboard	f	2026-03-11 05:42:45.676848
21	5	Câu hỏi mới	Nguyễn Văn A đã đặt câu hỏi trong bài: Tăng huyết áp	/Course/Lesson/110#comments	f	2026-03-11 05:43:37.097889
22	3	Câu hỏi của bạn đã được trả lời	Nguyễn Văn A đã trả lời câu hỏi của bạn trong bài: Tăng huyết áp	/Course/Lesson/110#qa	f	2026-03-11 05:44:25.524674
23	27	Chứng chỉ đã được duyệt	Chứng chỉ "Chứng chỉ chuyên môn" của bạn đã được quản trị viên phê duyệt.	/Expert/Qualifications	f	2026-03-11 05:49:19.353062
25	5	Câu hỏi mới	Nguyễn Hữu Huy đã đặt câu hỏi trong bài: Bài 3: Bài tập tình huống thực tế số 1	/Course/Lesson/3#comments	f	2026-03-11 07:31:25.50028
26	29	Chứng chỉ đã được duyệt	Chứng chỉ "Chứng chỉ chuyên môn" của bạn đã được quản trị viên phê duyệt.	/Expert/Qualifications	f	2026-03-11 09:54:12.96761
27	30	Chứng chỉ đã được duyệt	Chứng chỉ "Chứng chỉ chuyên môn" của bạn đã được quản trị viên phê duyệt.	/Expert/Qualifications	f	2026-03-14 04:33:38.079329
30	5	Câu hỏi mới	Nguyễn Hữu Huy đã đặt câu hỏi trong bài: Bài 1: Nguyên tắc 3C (Check - Call - Care)	/Course/Lesson/1#comments	f	2026-03-14 05:05:30.332979
24	18	Câu hỏi của bạn đã được trả lời	Nguyễn Hữu Huy đã trả lời câu hỏi của bạn trong bài: Bài 2: Đánh giá nhanh tình trạng nạn nhân (DRABC)	/Course/Lesson/2#qa	t	2026-03-11 07:24:12.534514
28	18	Phản hồi mới	Nguyễn Hữu Huy đã phản hồi trong bài: Bài 3: Bài tập tình huống thực tế số 1	/Course/Lesson/3#qa	t	2026-03-14 05:03:43.366843
29	18	Phản hồi mới	Nguyễn Hữu Huy đã phản hồi trong bài: Bài 3: Bài tập tình huống thực tế số 1	/Course/Lesson/3#qa	t	2026-03-14 05:03:45.719143
31	18	Gói đăng ký hết hạn	Gói 'Starter' của bạn đã hết hạn vào ngày 02/04/2026. Hãy gia hạn để tiếp tục sử dụng!	/Subscription	f	2026-04-03 07:04:22.783778
\.


--
-- Data for Name: plan_courses; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.plan_courses (plan_id, course_id) FROM stdin;
\.


--
-- Data for Name: plans; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.plans (id, name, price, description, features, duration_months, durationvalue, durationunit, duration_value, duration_unit) FROM stdin;
1	Starter	50000.00	Gói cơ bản cho doanh nghiệp nhỏ	Truy cập khóa học cơ bản,Chứng chỉ hoàn thành,Hỗ trợ qua email	12	1	Month	1	Month
2	Professional	159000.00	Giải pháp toàn diện cho doanh nghiệp	Tất cả tính năng Starter,Khóa học nâng cao,Quản lý nhân viên,Báo cáo chi tiết,Hỗ trợ 24/7	12	1	Month	1	Month
\.


--
-- Data for Name: qualifications; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.qualifications (id, title, description, certificate_url, issued_at, user_id, status, admin_comment) FROM stdin;
1	Chứng chỉ chuyên môn	Chức danh: Bác si - Nơi làm làm việc: Bệnh viện 198 - 2 năm kinh nghiệm	/images/certificates/0700b4e5-99c9-447e-b2b4-e672406d9f6d_chung_chi_y_te_sieu_chuan.png	2026-03-10 10:22:26.999015	25	Approved	\N
3	Chứng chỉ chuyên môn	Chức danh: Bác sĩ - Nơi làm làm việc: Bệnh Viện Nhi Đồng - 10 năm kinh nghiệm	/images/certificates/12ef3c0d-9369-412d-a562-b9138311246b_CNTest3.jpg	2026-03-11 05:48:34.357572	27	Approved	\N
4	Chứng chỉ chuyên môn	Chức danh: Bác sĩ - Nơi làm làm việc: Bệnh Viện Nhi Đồng - 5 năm kinh nghiệm	/images/certificates/391fa4c6-f781-4e5e-8b8b-4283d3f41d21_CNTest2.jpg	2026-03-11 09:53:23.204898	29	Approved	\N
5	Chứng chỉ chuyên môn	Chức danh: Bác sĩ - Nơi làm làm việc: Bệnh Viện Nhi Đồng - 5 năm kinh nghiệm	/images/certificates/6944d72a-99ee-47ee-8dd3-e143b333b0a5_CNTest2.jpg	2026-03-13 15:29:47.572712	30	Approved	\N
6	Chứng chỉ chuyên môn	Chức danh: Bác SĨ - Nơi làm làm việc: Bệnh Viện Nhi Đồng - 3 năm kinh nghiệm	/images/certificates/16e4fe66-7f7f-4a75-811d-61aeb000cfa4_CNTest3.jpg	2026-03-18 04:21:42.823992	31	Pending	\N
\.


--
-- Data for Name: roles; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.roles (id, role_name) FROM stdin;
1	Admin
2	Expert
3	User
\.


--
-- Data for Name: settings; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.settings (key, value, description, "group") FROM stdin;
SiteName	FirstAid+	Tên của website	General
MaintenanceMode	false	Chế độ bảo trì	General
SupportEmail	support@firstaid.vn	Email hỗ trợ	General
MaxUploadSize	10	Dung lượng upload tối đa (MB)	System
\.


--
-- Data for Name: testimonials; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.testimonials (id, student_name, student_role, content, rating) FROM stdin;
1	Dr. Nguyễn Minh Anh	Bác sĩ - BV Đa khoa QT	Khóa BLS rất chuyên nghiệp, nội dung cập nhật theo chuẩn AHA 2020.	5
\.


--
-- Data for Name: transactions; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.transactions (id, user_id, plan_id, amount, order_description, vnp_txn_ref, vnp_transaction_no, status, created_at, momo_order_id, momo_trans_id, payment_method) FROM stdin;
1	4	1	50000.00	Mua goi Starter cho user amshuuhuy10	639060158846637717	\N	Pending	2026-02-06 23:04:44.663801	\N	\N	VnPay
2	4	2	159000.00	Mua goi Professional cho user amshuuhuy10	639060165831178994	\N	Pending	2026-02-06 23:16:23.117928	\N	\N	VnPay
3	4	1	50000.00	Mua goi Starter cho user amshuuhuy10	639061975860923826	\N	Pending	2026-02-08 18:33:06.092425	\N	\N	VnPay
4	4	1	50000.00	Mua goi Starter cho user amshuuhuy10	639061975961294475	\N	Pending	2026-02-08 18:33:16.129514	\N	\N	VnPay
5	4	1	50000.00	Mua goi Starter cho user amshuuhuy10	639062051819614660	\N	Pending	2026-02-08 20:39:41.961437	\N	\N	VnPay
6	4	1	50000.00	Mua goi Starter cho user amshuuhuy10	\N	\N	Pending	2026-02-08 20:39:46.159425	1e2602aa-394b-44cd-bd5f-ce2adf2a672e	\N	Momo
7	4	1	50000.00	Mua goi Starter cho user amshuuhuy10	\N	\N	Pending	2026-02-08 20:39:48.922398	5be16679-8910-47e6-ac2d-de6ba6326253	\N	Momo
8	4	1	50000.00	Mua goi Starter cho user amshuuhuy10	\N	\N	Pending	2026-02-08 20:44:15.596163	796772d9-5aed-46d3-8b92-707cb7865692	\N	Momo
9	9	1	50000.00	Mua goi Starter cho user dropgcsgo@gmail.com	639076185614928203	\N	Pending	2026-02-25 05:16:01.4928	\N	\N	VnPay
10	9	1	50000.00	Mua goi Starter cho user dropgcsgo@gmail.com	\N	\N	Pending	2026-02-25 05:17:16.565553	f125609a-e798-4c1b-aff6-23195c946237	\N	Momo
11	9	1	50000.00	Mua goi Starter cho user dropgcsgo@gmail.com	\N	\N	Pending	2026-02-25 05:17:16.932943	17153b04-dc25-4c82-9c47-7e2acf78efaf	\N	Momo
12	9	1	50000.00	Mua goi Starter cho user dropgcsgo@gmail.com	639076186425063610	15430643	Success	2026-02-25 05:17:22.50636	\N	\N	VnPay
13	10	1	50000.00	Mua goi Starter cho user Truong13032611@gmail.com	639076223854186929	15430664	Success	2026-02-25 06:19:45.418674	\N	\N	VnPay
14	10	1	50000.00	Mua goi Starter cho user Truong13032611@gmail.com	639076338331385330	\N	Pending	2026-02-25 09:30:33.138518	\N	\N	VnPay
15	10	1	50000.00	Mua goi Starter cho user Truong13032611@gmail.com	639076342901672604	15431103	Success	2026-02-25 09:38:10.167245	\N	\N	VnPay
16	13	1	50000.00	Mua goi Starter cho user truong13032611@gmail.com	639076349757253628	15431143	Success	2026-02-25 09:49:35.725343	\N	\N	VnPay
17	4	2	159000.00	Mua goi Professional cho user amshuuhuy10	639078257095457511	\N	Pending	2026-02-27 14:48:29.54574	\N	\N	VnPay
18	6	1	50000.00	Mua goi Starter cho user huypersonal0312@gmail.com	639081706659643930	\N	Success	2026-03-03 14:37:45.964376	\N	\N	BankTransfer
19	4	1	50000.00	Mua goi Starter cho user amshuuhuy10	\N	\N	Pending	2026-03-04 07:44:34.575654	6dd10b03-2e1f-42b7-a385-382dbec68931	\N	Momo
20	4	1	50000.00	Mua goi Starter cho user amshuuhuy10	639082322797222404	15438160	Success	2026-03-04 07:44:39.722235	\N	\N	VnPay
21	18	1	50000.00	Mua goi Starter cho user amshuuhuy10@gmail.com	\N	\N	Success	2026-03-05 13:45:27.495765	\N	\N	PayOS
22	3	1	50000.00	Mua goi Starter cho user user1	\N	\N	Success	2026-03-06 15:13:14.453743	\N	\N	PayOS
23	18	1	50000.00	Mua goi Starter cho user amshuuhuy10@gmail.com	\N	\N	Pending	2026-03-10 09:07:28.253364	\N	\N	PayOS
24	28	1	50000.00	Mua goi Starter cho user truong13032611	\N	\N	Pending	2026-03-11 05:58:09.627433	\N	\N	PayOS
25	5	1	50000.00	Mua goi Starter cho user expert	\N	\N	Success	2026-03-11 09:45:18.980575	\N	\N	PayOS
26	32	1	50000.00	Mua goi Starter cho user thuanvmse172569	639100324836268957	\N	Pending	2026-03-25 03:48:03.626877	\N	\N	VnPay
27	32	1	50000.00	Mua goi Starter cho user thuanvmse172569	\N	\N	Pending	2026-03-25 03:48:44.811315	52105aa9-c524-490f-a939-22761028d311	\N	Momo
28	32	1	50000.00	Mua goi Starter cho user thuanvmse172569	\N	\N	Success	2026-03-25 03:48:52.158096	\N	\N	PayOS
29	33	1	50000.00	Mua goi Starter cho user thuanvo332003	\N	\N	Success	2026-03-25 03:51:06.767671	\N	\N	PayOS
30	34	1	50000.00	Mua goi Starter cho user tinbtse182286	\N	\N	Pending	2026-03-25 04:19:07.494165	f6aa4ab6-1755-4003-a189-1b2cf3c1eec0	\N	Momo
31	34	1	50000.00	Mua goi Starter cho user tinbtse182286	\N	\N	Pending	2026-03-25 04:19:15.318907	1e6ae67c-0bc2-40ba-8a35-3c8d52751b7f	\N	Momo
32	34	1	50000.00	Mua goi Starter cho user tinbtse182286	\N	\N	Success	2026-03-25 04:19:21.391567	\N	\N	PayOS
33	35	1	50000.00	Mua goi Starter cho user vinhlcse184764	\N	\N	Pending	2026-03-25 04:25:12.217312	\N	\N	PayOS
34	35	1	50000.00	Mua goi Starter cho user vinhlcse184764	\N	\N	Pending	2026-03-25 04:25:26.397627	\N	\N	PayOS
35	35	1	50000.00	Mua goi Starter cho user vinhlcse184764	\N	\N	Success	2026-03-25 04:29:38.86172	\N	\N	PayOS
36	36	1	50000.00	Mua goi Starter cho user dinhthinh1401	\N	\N	Pending	2026-03-25 08:09:48.42776	ae878845-f51c-4879-ae9f-df66d83266de	\N	Momo
37	36	1	50000.00	Mua goi Starter cho user dinhthinh1401	639100481942487919	\N	Pending	2026-03-25 08:09:54.248791	\N	\N	VnPay
38	36	1	50000.00	Mua goi Starter cho user dinhthinh1401	639100482636305865	\N	Pending	2026-03-25 08:11:03.630586	\N	\N	VnPay
39	36	1	50000.00	Mua goi Starter cho user dinhthinh1401	\N	\N	Pending	2026-03-25 08:15:45.272914	\N	\N	PayOS
40	36	1	50000.00	Mua goi Starter cho user dinhthinh1401	\N	\N	Pending	2026-03-25 08:16:40.869514	1b892dc1-f5b4-4c6e-86e1-96f49831c8cf	\N	Momo
41	36	1	50000.00	Mua goi Starter cho user dinhthinh1401	\N	\N	Success	2026-03-25 08:17:00.798047	\N	\N	PayOS
42	18	1	50000.00	Mua goi Starter cho user amshuuhuy10@gmail.com	\N	\N	Pending	2026-03-25 12:21:00.222353	\N	\N	PayOS
43	18	1	50000.00	Mua goi Starter cho user amshuuhuy10@gmail.com	\N	\N	Pending	2026-04-03 07:51:24.401933	\N	\N	PayOS
\.


--
-- Data for Name: user_game_progress; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.user_game_progress (id, user_id, situation_id, is_completed, score_earned, completed_at) FROM stdin;
\.


--
-- Data for Name: user_lesson_progress; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.user_lesson_progress (id, user_id, lesson_id, time_spent_seconds, is_completed, last_accessed) FROM stdin;
16	18	1	50	t	2026-03-18 01:47:25.881342
25	3	1	0	f	2026-03-18 04:23:54.799899
17	18	80	0	f	2026-03-11 03:40:22.943092
22	18	3	300	f	2026-04-03 07:59:42.113701
23	5	111	15	f	2026-03-11 09:30:34.462258
24	5	3	0	f	2026-03-12 14:39:11.342603
21	18	2	60	t	2026-03-14 04:55:47.462568
\.


--
-- Data for Name: user_subscriptions; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.user_subscriptions (id, user_id, plan_id, start_date, end_date, status) FROM stdin;
7	3	1	2026-03-06 15:14:37.711812	2026-04-06 15:14:37.711827	Active
8	5	1	2026-03-11 09:45:45.724903	2026-04-11 09:45:45.724933	Active
9	32	1	2026-03-25 03:49:18.971283	2026-04-25 03:49:18.971297	Active
10	33	1	2026-03-25 03:51:26.892303	2026-04-25 03:51:26.892303	Active
11	34	1	2026-03-25 04:19:56.6266	2026-04-25 04:19:56.6266	Active
12	35	1	2026-03-25 04:30:47.794764	2026-04-25 04:30:47.794764	Active
13	36	1	2026-03-25 08:17:29.055862	2026-04-25 08:17:29.055862	Active
6	18	1	2026-03-05 13:48:28.330732	2026-04-02 13:48:28.330745	Expired
\.


--
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.users (id, username, email, password_hash, full_name, role_id, phone, address, bio, avatar_url, reset_token, reset_token_expiry, created_at, is_email_confirmed, email_confirmation_token) FROM stdin;
29	truonghdse183406@fpt.edu.vn	truonghdse183406@fpt.edu.vn	$2a$11$/vf.szJ4Qx6tixHY1Wr2TO6/TeTBLXdpbaSwZmJowmOjYmrxMRZTS	Nguyễn Thị Thảo 	2	\N	\N	Chức danh: Bác sĩ - Nơi làm làm việc: Bệnh Viện Nhi Đồng - 5 năm kinh nghiệm	\N	\N	\N	2026-03-11 09:53:22.879679	t	\N
1	admin	admin@firstaid.vn	$2a$12$q/Pys1WEaEpSOJZkIYlx3./FeBRKRpAHdZ72GEP7RQG/cOfHFl4c6	System Administrator	1	\N	\N	\N	\N	\N	\N	2026-01-22 10:04:40.497364	t	\N
2	expert1	mai.nguyen@firstaid.vn	$2a$12$q/Pys1WEaEpSOJZkIYlx3./FeBRKRpAHdZ72GEP7RQG/cOfHFl4c6	BS. Nguyễn Thị Mai	2	\N	\N	\N	\N	6a07d733-81c0-41bb-b39d-61168e862f4c	2026-02-05 14:26:17.698365	2026-01-22 10:04:40.497364	t	\N
3	user1	hocvien@gmail.com	$2a$12$q/Pys1WEaEpSOJZkIYlx3./FeBRKRpAHdZ72GEP7RQG/cOfHFl4c6	Nguyễn Văn A	3	\N	\N	\N	\N	\N	\N	2026-01-22 10:04:40.497364	t	\N
5	expert	expert@firstaidplus.com	$2a$12$q/Pys1WEaEpSOJZkIYlx3./FeBRKRpAHdZ72GEP7RQG/cOfHFl4c6	Chuyên gia có chứng chỉ	2	\N	\N	\N	/images/avatars/2121647a-4c9c-447e-84c7-357377a67d6b_image (6).png	\N	\N	2026-02-04 14:24:09.275817	t	\N
30	ngoquynhnhu01012019@gmail.com	ngoquynhnhu01012019@gmail.com	$2a$11$Gdzt7sHeHxkxmOe60ncjzeramFbvwrNg3M.FNyBUAaXBYrA1K5EdG	Nguyễn Thị Thảo 	2	\N	\N	Chức danh: Bác sĩ - Nơi làm làm việc: Bệnh Viện Nhi Đồng - 5 năm kinh nghiệm	\N	\N	\N	2026-03-13 15:29:47.386238	f	06d110c3-a267-4962-9569-bdb0ca445e1a
25	vuongdinhthinh6112003@gmail.com	vuongdinhthinh6112003@gmail.com	$2a$11$KJEO.PMg8ftLbuR3c7FMFe2qh40J0Wz30sk.kBNZ/hLOz.z9tN3Ba	Nguyễn Hữu Huy	2	\N	\N	Chức danh: Bác si - Nơi làm làm việc: Bệnh viện 198 - 2 năm kinh nghiệm	\N	\N	\N	2026-03-10 10:22:26.846495	t	\N
31	Truong13032611@gmail.com	Truong13032611@gmail.com	$2a$11$vv/9COZm7uEUBAXA3GGEO.4TZ0cD/d9BwNSIfjXXCRknZF3vwyezW	Phạm Minh Tuấn	2	\N	\N	Chức danh: Bác SĨ - Nơi làm làm việc: Bệnh Viện Nhi Đồng - 3 năm kinh nghiệm	\N	\N	\N	2026-03-18 04:21:42.68659	f	06119da7-5481-4551-991a-fca09696465f
32	thuanvmse172569	thuanvmse172569@fpt.edu.vn	GOOGLE_AUTH_106369821701794028673	Vo Minh Thuan (K17 HCM)	3	\N	\N	\N	\N	\N	\N	2026-03-25 03:47:50.167543	t	\N
33	thuanvo332003	thuanvo332003@gmail.com	GOOGLE_AUTH_105705790154565500470	Thuận Võ	3	\N	\N	\N	\N	\N	\N	2026-03-25 03:50:57.789052	t	\N
18	amshuuhuy10@gmail.com	amshuuhuy10@gmail.com	$2a$11$GkAZIk.Y2HDKF7BGj/xJ.O1jXF98MujYBLVX4./v9oj8csknAVnzu	Nguyễn Hữu Huy	3	0862031203	Số nhà 36 ngõ 123	123	/images/avatars/325da2b4-e50e-4673-9cc5-2ac55f46e753_avatar1735176031764-17351760331641924544187.jpg	\N	\N	2026-03-05 13:43:40.843937	t	\N
34	tinbtse182286	tinbtse182286@fpt.edu.vn	GOOGLE_AUTH_108892169168748647095	Bien Trung Tin (K18 HCM)	3	\N	\N	\N	\N	\N	\N	2026-03-25 04:18:49.567674	t	\N
35	vinhlcse184764	vinhlcse184764@fpt.edu.vn	GOOGLE_AUTH_107631037213199487320	Le Cong Vinh (K18 HCM)	3	\N	\N	\N	\N	\N	\N	2026-03-25 04:24:47.275046	t	\N
27	vominhthuan332003@gmail.com	vominhthuan332003@gmail.com	$2a$11$eDPA0g.34Zhfu4/.U7jKB.oK2vuTxgQf0323BmhzF/LDcyf9rbb8i	Phạm Minh Tuấn	2	\N	\N	Chức danh: Bác sĩ - Nơi làm làm việc: Bệnh Viện Nhi Đồng - 10 năm kinh nghiệm	\N	\N	\N	2026-03-11 05:48:34.252152	t	\N
28	truong13032611	truong13032611@gmail.com	GOOGLE_AUTH_111343192749761401210	trường huỳnh	3	\N	\N	\N	\N	\N	\N	2026-03-11 05:57:57.81094	t	\N
36	dinhthinh1401	dinhthinh1401@gmail.com	GOOGLE_AUTH_107934724995799502226	Thinh Nguyen	3	\N	\N	\N	\N	\N	\N	2026-03-25 08:08:50.16822	t	\N
\.


--
-- Name: carts_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.carts_id_seq', 1, false);


--
-- Name: comment_reactions_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.comment_reactions_id_seq', 3, true);


--
-- Name: course_lessons_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.course_lessons_id_seq', 111, true);


--
-- Name: course_objectives_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.course_objectives_id_seq', 82, true);


--
-- Name: course_syllabus_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.course_syllabus_id_seq', 76, true);


--
-- Name: courses_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.courses_id_seq', 36, true);


--
-- Name: enrollments_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.enrollments_id_seq', 14, true);


--
-- Name: family_course_categories_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.family_course_categories_id_seq', 5, true);


--
-- Name: feedbacks_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.feedbacks_id_seq', 20, true);


--
-- Name: game_category_experts_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.game_category_experts_id_seq', 13, true);


--
-- Name: game_options_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.game_options_id_seq', 34, true);


--
-- Name: game_situations_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.game_situations_id_seq', 10, true);


--
-- Name: lesson_comments_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.lesson_comments_id_seq', 16, true);


--
-- Name: lesson_notes_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.lesson_notes_id_seq', 3, true);


--
-- Name: medical_records_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.medical_records_id_seq', 5, true);


--
-- Name: messages_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.messages_id_seq', 15, true);


--
-- Name: notifications_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.notifications_id_seq', 31, true);


--
-- Name: plans_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.plans_id_seq', 2, true);


--
-- Name: qualifications_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.qualifications_id_seq', 6, true);


--
-- Name: roles_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.roles_id_seq', 6, true);


--
-- Name: testimonials_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.testimonials_id_seq', 1, true);


--
-- Name: transactions_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.transactions_id_seq', 43, true);


--
-- Name: user_game_progress_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.user_game_progress_id_seq', 4, true);


--
-- Name: user_lesson_progress_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.user_lesson_progress_id_seq', 25, true);


--
-- Name: user_subscriptions_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.user_subscriptions_id_seq', 13, true);


--
-- Name: users_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.users_id_seq', 36, true);


--
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- Name: medical_records PK_medical_records; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.medical_records
    ADD CONSTRAINT "PK_medical_records" PRIMARY KEY (id);


--
-- Name: carts carts_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.carts
    ADD CONSTRAINT carts_pkey PRIMARY KEY (id);


--
-- Name: comment_reactions comment_reactions_comment_id_user_id_reaction_type_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.comment_reactions
    ADD CONSTRAINT comment_reactions_comment_id_user_id_reaction_type_key UNIQUE (comment_id, user_id, reaction_type);


--
-- Name: comment_reactions comment_reactions_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.comment_reactions
    ADD CONSTRAINT comment_reactions_pkey PRIMARY KEY (id);


--
-- Name: course_lessons course_lessons_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.course_lessons
    ADD CONSTRAINT course_lessons_pkey PRIMARY KEY (id);


--
-- Name: course_objectives course_objectives_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.course_objectives
    ADD CONSTRAINT course_objectives_pkey PRIMARY KEY (id);


--
-- Name: course_syllabus course_syllabus_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.course_syllabus
    ADD CONSTRAINT course_syllabus_pkey PRIMARY KEY (id);


--
-- Name: courses courses_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.courses
    ADD CONSTRAINT courses_pkey PRIMARY KEY (id);


--
-- Name: enrollments enrollments_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.enrollments
    ADD CONSTRAINT enrollments_pkey PRIMARY KEY (id);


--
-- Name: family_course_categories family_course_categories_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.family_course_categories
    ADD CONSTRAINT family_course_categories_pkey PRIMARY KEY (id);


--
-- Name: feedbacks feedbacks_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.feedbacks
    ADD CONSTRAINT feedbacks_pkey PRIMARY KEY (id);


--
-- Name: game_category_experts game_category_experts_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.game_category_experts
    ADD CONSTRAINT game_category_experts_pkey PRIMARY KEY (id);


--
-- Name: game_options game_options_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.game_options
    ADD CONSTRAINT game_options_pkey PRIMARY KEY (id);


--
-- Name: game_situations game_situations_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.game_situations
    ADD CONSTRAINT game_situations_pkey PRIMARY KEY (id);


--
-- Name: lesson_comments lesson_comments_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.lesson_comments
    ADD CONSTRAINT lesson_comments_pkey PRIMARY KEY (id);


--
-- Name: lesson_notes lesson_notes_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.lesson_notes
    ADD CONSTRAINT lesson_notes_pkey PRIMARY KEY (id);


--
-- Name: messages messages_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.messages
    ADD CONSTRAINT messages_pkey PRIMARY KEY (id);


--
-- Name: notifications notifications_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.notifications
    ADD CONSTRAINT notifications_pkey PRIMARY KEY (id);


--
-- Name: plan_courses plan_courses_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.plan_courses
    ADD CONSTRAINT plan_courses_pkey PRIMARY KEY (plan_id, course_id);


--
-- Name: plans plans_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.plans
    ADD CONSTRAINT plans_pkey PRIMARY KEY (id);


--
-- Name: qualifications qualifications_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.qualifications
    ADD CONSTRAINT qualifications_pkey PRIMARY KEY (id);


--
-- Name: roles roles_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.roles
    ADD CONSTRAINT roles_pkey PRIMARY KEY (id);


--
-- Name: roles roles_role_name_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.roles
    ADD CONSTRAINT roles_role_name_key UNIQUE (role_name);


--
-- Name: settings settings_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.settings
    ADD CONSTRAINT settings_pkey PRIMARY KEY (key);


--
-- Name: testimonials testimonials_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.testimonials
    ADD CONSTRAINT testimonials_pkey PRIMARY KEY (id);


--
-- Name: transactions transactions_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.transactions
    ADD CONSTRAINT transactions_pkey PRIMARY KEY (id);


--
-- Name: user_game_progress user_game_progress_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.user_game_progress
    ADD CONSTRAINT user_game_progress_pkey PRIMARY KEY (id);


--
-- Name: user_lesson_progress user_lesson_progress_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.user_lesson_progress
    ADD CONSTRAINT user_lesson_progress_pkey PRIMARY KEY (id);


--
-- Name: user_subscriptions user_subscriptions_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.user_subscriptions
    ADD CONSTRAINT user_subscriptions_pkey PRIMARY KEY (id);


--
-- Name: users users_email_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_email_key UNIQUE (email);


--
-- Name: users users_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (id);


--
-- Name: users users_username_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_username_key UNIQUE (username);


--
-- Name: IX_medical_records_user_id; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_medical_records_user_id" ON public.medical_records USING btree (user_id);


--
-- Name: idx_lesson_notes_lesson_id; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_lesson_notes_lesson_id ON public.lesson_notes USING btree (lesson_id);


--
-- Name: idx_lesson_notes_user_id; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_lesson_notes_user_id ON public.lesson_notes USING btree (user_id);


--
-- Name: ix_game_category_experts_category_id; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX ix_game_category_experts_category_id ON public.game_category_experts USING btree (category_id);


--
-- Name: ix_game_category_experts_expert_id; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX ix_game_category_experts_expert_id ON public.game_category_experts USING btree (expert_id);


--
-- Name: ix_user_lesson_progress_lesson_id; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX ix_user_lesson_progress_lesson_id ON public.user_lesson_progress USING btree (lesson_id);


--
-- Name: ix_user_lesson_progress_user_id; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX ix_user_lesson_progress_user_id ON public.user_lesson_progress USING btree (user_id);


--
-- Name: medical_records FK_medical_records_users_user_id; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.medical_records
    ADD CONSTRAINT "FK_medical_records_users_user_id" FOREIGN KEY (user_id) REFERENCES public.users(id) ON DELETE CASCADE;


--
-- Name: carts carts_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.carts
    ADD CONSTRAINT carts_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.users(id) ON DELETE CASCADE;


--
-- Name: comment_reactions comment_reactions_comment_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.comment_reactions
    ADD CONSTRAINT comment_reactions_comment_id_fkey FOREIGN KEY (comment_id) REFERENCES public.lesson_comments(id) ON DELETE CASCADE;


--
-- Name: comment_reactions comment_reactions_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.comment_reactions
    ADD CONSTRAINT comment_reactions_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.users(id) ON DELETE CASCADE;


--
-- Name: course_lessons course_lessons_syllabus_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.course_lessons
    ADD CONSTRAINT course_lessons_syllabus_id_fkey FOREIGN KEY (syllabus_id) REFERENCES public.course_syllabus(id) ON DELETE CASCADE;


--
-- Name: course_objectives course_objectives_course_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.course_objectives
    ADD CONSTRAINT course_objectives_course_id_fkey FOREIGN KEY (course_id) REFERENCES public.courses(id) ON DELETE CASCADE;


--
-- Name: course_syllabus course_syllabus_course_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.course_syllabus
    ADD CONSTRAINT course_syllabus_course_id_fkey FOREIGN KEY (course_id) REFERENCES public.courses(id) ON DELETE CASCADE;


--
-- Name: enrollments enrollments_course_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.enrollments
    ADD CONSTRAINT enrollments_course_id_fkey FOREIGN KEY (course_id) REFERENCES public.courses(id) ON DELETE CASCADE;


--
-- Name: enrollments enrollments_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.enrollments
    ADD CONSTRAINT enrollments_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.users(id) ON DELETE CASCADE;


--
-- Name: feedbacks fk_feedbacks_course; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.feedbacks
    ADD CONSTRAINT fk_feedbacks_course FOREIGN KEY (course_id) REFERENCES public.courses(id) ON DELETE CASCADE;


--
-- Name: feedbacks fk_feedbacks_user; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.feedbacks
    ADD CONSTRAINT fk_feedbacks_user FOREIGN KEY (user_id) REFERENCES public.users(id) ON DELETE CASCADE;


--
-- Name: game_category_experts fk_gce_category; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.game_category_experts
    ADD CONSTRAINT fk_gce_category FOREIGN KEY (category_id) REFERENCES public.family_course_categories(id) ON DELETE CASCADE;


--
-- Name: game_category_experts fk_gce_expert; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.game_category_experts
    ADD CONSTRAINT fk_gce_expert FOREIGN KEY (expert_id) REFERENCES public.users(id) ON DELETE CASCADE;


--
-- Name: lesson_comments fk_lesson_comments_lessons; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.lesson_comments
    ADD CONSTRAINT fk_lesson_comments_lessons FOREIGN KEY (lesson_id) REFERENCES public.course_lessons(id) ON DELETE CASCADE;


--
-- Name: lesson_comments fk_lesson_comments_parent; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.lesson_comments
    ADD CONSTRAINT fk_lesson_comments_parent FOREIGN KEY (parent_id) REFERENCES public.lesson_comments(id);


--
-- Name: lesson_comments fk_lesson_comments_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.lesson_comments
    ADD CONSTRAINT fk_lesson_comments_users FOREIGN KEY (user_id) REFERENCES public.users(id) ON DELETE CASCADE;


--
-- Name: lesson_notes fk_lesson_notes_lessons; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.lesson_notes
    ADD CONSTRAINT fk_lesson_notes_lessons FOREIGN KEY (lesson_id) REFERENCES public.course_lessons(id) ON DELETE CASCADE;


--
-- Name: lesson_notes fk_lesson_notes_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.lesson_notes
    ADD CONSTRAINT fk_lesson_notes_users FOREIGN KEY (user_id) REFERENCES public.users(id) ON DELETE CASCADE;


--
-- Name: notifications fk_notifications_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.notifications
    ADD CONSTRAINT fk_notifications_users FOREIGN KEY (user_id) REFERENCES public.users(id) ON DELETE CASCADE;


--
-- Name: plan_courses fk_plancourses_course; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.plan_courses
    ADD CONSTRAINT fk_plancourses_course FOREIGN KEY (course_id) REFERENCES public.courses(id) ON DELETE CASCADE;


--
-- Name: plan_courses fk_plancourses_plan; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.plan_courses
    ADD CONSTRAINT fk_plancourses_plan FOREIGN KEY (plan_id) REFERENCES public.plans(id) ON DELETE CASCADE;


--
-- Name: user_lesson_progress fk_user_lesson_progress_course_lessons; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.user_lesson_progress
    ADD CONSTRAINT fk_user_lesson_progress_course_lessons FOREIGN KEY (lesson_id) REFERENCES public.course_lessons(id) ON DELETE CASCADE;


--
-- Name: user_lesson_progress fk_user_lesson_progress_users; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.user_lesson_progress
    ADD CONSTRAINT fk_user_lesson_progress_users FOREIGN KEY (user_id) REFERENCES public.users(id) ON DELETE CASCADE;


--
-- Name: game_options game_options_situation_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.game_options
    ADD CONSTRAINT game_options_situation_id_fkey FOREIGN KEY (situation_id) REFERENCES public.game_situations(id);


--
-- Name: game_situations game_situations_category_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.game_situations
    ADD CONSTRAINT game_situations_category_id_fkey FOREIGN KEY (category_id) REFERENCES public.family_course_categories(id);


--
-- Name: messages messages_receiver_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.messages
    ADD CONSTRAINT messages_receiver_id_fkey FOREIGN KEY (receiver_id) REFERENCES public.users(id) ON DELETE CASCADE;


--
-- Name: messages messages_sender_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.messages
    ADD CONSTRAINT messages_sender_id_fkey FOREIGN KEY (sender_id) REFERENCES public.users(id) ON DELETE CASCADE;


--
-- Name: qualifications qualifications_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.qualifications
    ADD CONSTRAINT qualifications_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.users(id) ON DELETE CASCADE;


--
-- Name: user_game_progress user_game_progress_situation_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.user_game_progress
    ADD CONSTRAINT user_game_progress_situation_id_fkey FOREIGN KEY (situation_id) REFERENCES public.game_situations(id);


--
-- Name: user_game_progress user_game_progress_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.user_game_progress
    ADD CONSTRAINT user_game_progress_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.users(id);


--
-- Name: user_subscriptions user_subscriptions_plan_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.user_subscriptions
    ADD CONSTRAINT user_subscriptions_plan_id_fkey FOREIGN KEY (plan_id) REFERENCES public.plans(id) ON DELETE CASCADE;


--
-- Name: user_subscriptions user_subscriptions_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.user_subscriptions
    ADD CONSTRAINT user_subscriptions_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.users(id) ON DELETE CASCADE;


--
-- Name: users users_role_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_role_id_fkey FOREIGN KEY (role_id) REFERENCES public.roles(id);


--
-- PostgreSQL database dump complete
--

\unrestrict QW4DDy55SXhuRdKDvB6qLMFC05ubjiWagOVHhzlqd7YSXFCRqbETJklEV7BDgAV

