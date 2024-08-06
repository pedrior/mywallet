CREATE FUNCTION set_created_at()
    RETURNS TRIGGER AS
$$
BEGIN
    NEW.created_at = NOW() AT TIME ZONE 'UTC';
    RETURN NEW;
END;
$$
    LANGUAGE plpgsql;

CREATE FUNCTION set_updated_at()
    RETURNS TRIGGER AS
$$
BEGIN
    NEW.updated_at = NOW() AT TIME ZONE 'UTC';
    RETURN NEW;
END;
$$
    LANGUAGE plpgsql;