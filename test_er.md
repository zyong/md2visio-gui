erDiagram

  apps { 
    uuid id pk "主键"
    uuid tenant_id "租户id"
    string name "应用名"
    string description  ""
    string mode ""
    string icon_type ""
    string icon ""
    uuid app_model_config_id ""
    uuid workflow_id "工作流id"
    string status "状态"
    bool enable_site "应用站点"
    bool enable_api "应用api"
    bool is_public "是否公开"
    bool is_universal "是否？"
    datetime created_at ""
    datetime updated_at ""
    string created_by ""
    string updated_by ""
  }

  conversation {
    uuid id pk "主键"
    uuid app_id "应用id"
    string model_provider "模型厂商"
    string model_id "model_id"
    string name "会话名"
    string summary "摘要"
    string introduction "介绍"
    string system_introduction "系统描述"
    string status "状态"
    string invoke_from ""
    string from_source ""
    uuid   from_end_user_id ""
    uuid   from_account_id ""
    datetime created_at ""
    datetime updated_at ""
    bool is_deleted ""
  }

  messages {
    uuid id pk "主键"
    uuid app_id "应用id"
    string model_provider "模型厂商"
    string model_id "model_id"
    uuid conversation_id "回话id"
    string query "问题"
    json message "消息"
    string answer "回复"
    int answer_tokens "使用tokens"
    uuid parent_message_id "父消息id" 
    string status ""
    string error ""
    string invoke_from ""
    string from_source ""
    uuid   from_end_user_id ""
    uuid   from_account_id ""
    datetime created_at ""
    datetime updated_at ""
    uuid workflow_run_id "工作流id"
  }

  message_files {
    uuid id pk ""
    uuid message_id ""
    string type "文件类型"
    uuid upload_file_id ""
    uuid created_by "创建人"
    datetime created_at ""
    datetime updated_at ""
  }
  message_feedbacks {
    uuid id pk ""
    uuid app_id ""
    uuid conversation_id ""
    uuid message_id ""
    string rating ""
    datetime created_at ""
    datetime updated_at ""
  }
  
  accounts ||..o{ tenants : OneToOne
  workflow ||..o{ tenants : OneToOne
    
  
  apps ||..o{ app_model_configs : OneToOne
  apps ||..o{ tenants: OneToOne
  apps ||..o{ workflow : OneToMany
  app_model_configs ||..o{ dataset : OneToMany
  app_model_configs ||..o{ tool_api_provider : OneToMany
  apps ||..o{ conversation : OneToOne
  conversation ||..o{ messages : OneToOne
  messages ||..o{ message_files : OneToMany
  messages ||..o{ message_feedbacks: OneToOne
  messages ||..o{ message_like: OneToOne