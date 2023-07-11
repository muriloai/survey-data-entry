CREATE USER 'survey_user'@'localhost'
  IDENTIFIED WITH mysql_native_password BY 'sde2014';

GRANT ALL PRIVILEGES ON db_survey_data_entry.* TO 'survey_user'@'localhost';

FLUSH PRIVILEGES;