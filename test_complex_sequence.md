sequenceDiagram
  autonumber
  actor user as 用户
  participant frontend as 前端服务
  participant backend as 后端服务
  participant uc as 用户中心
  participant dify as dify服务
  participant llm as 大模型服务
  participant es as 检索服务
  participant rerank as 精排服务
  participant plugin as 插件服务



  activate user
  user ->> frontend: 打开问答Agent
  frontend ->> uc: 判断是否登录
  uc ->> frontend: 返回用户登录状态
  frontend ->> user: 展现问答Agent页面
  frontend ->> backend: 拉取问答历史会话
  backend ->> frontend: 返回历史会话数据
  frontend ->> user: 展现历史会话数据
  user ->> frontend: 提出问题
  frontend ->> backend: 调用大模型回答问题
  backend ->> backend: 解析请求，判断是否进行参数转换处理
  backend ->> dify: 请求处理问答
  dify ->> dify: 验证请求合法性，运行指定的app
opt 获取外部知识库
  dify ->> backend: 获取知识库数据
  backend ->> es: 获取知识库数据
  es ->> backend: 返回知识库数据
  backend ->> backend: 记录日志
  backend ->> rerank: 精排数据
  rerank ->> backend: 返回精排结果
  backend ->> backend: 过滤结果，记录日志
  backend ->> dify: 返回结果
end
  dify ->> llm: 请求大模型回答
  llm ->> dify: 返回结果
  dify ->> backend: 返回大模型问题
  backend ->> backend: 记录数据日志
  backend ->> frontend: 返回回答结果
  frontend ->> user: 展现大模型回答内容

opt 获取相关问题
  backend ->> backend: 判断是否开启相关问题设置
  backend ->> llm: 获取相关问题
  backend ->> backend: 记录相关问题日志
  backend ->> frontend: 返回相关问题
  frontend ->> user: 展现相关问题
end

opt 插件展现
  dify ->> dify: 判断是否开启插件设置
  dify ->> llm: 判断是否命中插件
  llm ->> dify: 确定结果
  dify ->> plugin: 调用插件
  plugin ->> dify: 返回结果
  dify ->> backend: 返回结果
  backend ->> frontend: 返回结果
  frontend ->> user: 展现插件
end
  deactivate user
