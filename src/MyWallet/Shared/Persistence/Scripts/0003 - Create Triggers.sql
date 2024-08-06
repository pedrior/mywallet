CREATE TRIGGER set_created_at_on_users
    BEFORE INSERT
    ON users
    FOR EACH ROW
EXECUTE FUNCTION set_created_at();

CREATE TRIGGER set_updated_at_on_users
    BEFORE UPDATE
    ON users
    FOR EACH ROW
EXECUTE FUNCTION set_updated_at();

CREATE TRIGGER set_created_at_on_categories
    BEFORE INSERT
    ON categories
    FOR EACH ROW
EXECUTE FUNCTION set_created_at();

CREATE TRIGGER set_updated_at_on_categories
    BEFORE UPDATE
    ON categories
    FOR EACH ROW
EXECUTE FUNCTION set_updated_at();