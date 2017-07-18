﻿/**************************************************************************
Copyright 2016 Carsten Gehling

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
**************************************************************************/
using System;
using System.Collections.Generic;

namespace StopWatch
{
    public class JiraClient
    {
        public bool SessionValid { get; private set; }

        public string ErrorMessage { get; private set; }

        #region public methods
        public JiraClient(IJiraApiRequestFactory jiraApiRequestFactory, IJiraApiRequester jiraApiRequester)
        {
            this.jiraApiRequestFactory = jiraApiRequestFactory;
            this.jiraApiRequester = jiraApiRequester;

            SessionValid = false;
            ErrorMessage = "";
        }


        public bool Authenticate(string username, string password)
        {
            SessionValid = false;

            var request = jiraApiRequestFactory.CreateAuthenticateRequest(username, password);
            try
            {
                jiraApiRequester.DoAuthenticatedRequest<object>(request);
                return true;
            }
            catch (RequestDeniedException)
            {
                ErrorMessage = jiraApiRequester.ErrorMessage;
                return false;
            }
        }


        public bool ValidateSession()
        {
            SessionValid = false;

            var request = jiraApiRequestFactory.CreateValidateSessionRequest();
            try
            {
                jiraApiRequester.DoAuthenticatedRequest<object>(request);
                SessionValid = true;
                return true;
            }
            catch (RequestDeniedException)
            {
                ErrorMessage = jiraApiRequester.ErrorMessage;
                return false;
            }
        }


        public List<Filter> GetFavoriteFilters()
        {
            var request = jiraApiRequestFactory.CreateGetFavoriteFiltersRequest();
            try
            {
                return jiraApiRequester.DoAuthenticatedRequest<List<Filter>>(request);
            }
            catch (RequestDeniedException)
            {
                return null;
            }
        }
        

        public SearchResult GetIssuesByJQL(string jql, int maxResults = 200)
        {
            var request = jiraApiRequestFactory.CreateGetIssuesByJQLRequest(jql, maxResults);
            try
            {
                return jiraApiRequester.DoAuthenticatedRequest<SearchResult>(request);
            }
            catch (RequestDeniedException)
            {
                return null;
            }
        }


        public string GetIssueSummary(string key)
        {
            var request = jiraApiRequestFactory.CreateGetIssueSummaryRequest(key);
            try
            {
                return jiraApiRequester.DoAuthenticatedRequest<Issue>(request).Fields.Summary;
            }
            catch (RequestDeniedException)
            {
                return "";
            }
        }

        public TimetrackingFields GetIssueTimetracking(string key)
        {
            var request = jiraApiRequestFactory.CreateGetIssueTimetrackingRequest(key);
            try
            {
                return jiraApiRequester.DoAuthenticatedRequest<Issue>(request).Fields.Timetracking;
            }
            catch (RequestDeniedException)
            {
                return null;
            }
        }


        public bool PostWorklog(string key, TimeSpan time, string comment, EstimateUpdateMethods estimateUpdateMethod, string estimateUpdateValue)
        {
            var started = DateTimeOffset.UtcNow.Subtract(time);
            var request = jiraApiRequestFactory.CreatePostWorklogRequest(key, started, time, comment, estimateUpdateMethod, estimateUpdateValue);
            try
            {
                jiraApiRequester.DoAuthenticatedRequest<object>(request);
                return true;
            }
            catch (RequestDeniedException)
            {
                return false;
            }
        }


        public bool PostComment(string key, string comment)
        {
            var request = jiraApiRequestFactory.CreatePostCommentRequest(key, comment);
            try
            {
                jiraApiRequester.DoAuthenticatedRequest<object>(request);
                return true;
            }
            catch (RequestDeniedException)
            {
                return false;
            }
        }


        public AvailableTransitions GetAvailableTransitions(string key)
        {
            var request = jiraApiRequestFactory.CreateGetAvailableTransitions(key);
            try
            {
                return jiraApiRequester.DoAuthenticatedRequest<AvailableTransitions>(request);
            }
            catch (RequestDeniedException)
            {
                return null;
            }
        }


        public bool DoTransition(string key, int transitionId)
        {
            var request = jiraApiRequestFactory.CreateDoTransition(key, transitionId);
            try
            {
                jiraApiRequester.DoAuthenticatedRequest<object>(request);
                return true;
            }
            catch (RequestDeniedException)
            {
                return false;
            }
        }


        public CreateIssueMeta GetCreateIssueMeta()
        {
            var request = jiraApiRequestFactory.CreateGetCreateIssueMetaRequest();
            try
            {
                return jiraApiRequester.DoAuthenticatedRequest<CreateIssueMeta>(request);
            }
            catch (RequestDeniedException)
            {
                return new CreateIssueMeta();
            }
        }


        public List<User> FindUsers(string searchPattern)
        {
            var request = jiraApiRequestFactory.CreateFindUsersRequest(searchPattern);
            try
            {
                return jiraApiRequester.DoAuthenticatedRequest<List<User>>(request);
            }
            catch (RequestDeniedException)
            {
                return new List<User>();
            }
        }

        public User GetMyself()
        {
            var request = jiraApiRequestFactory.CreateGetMyselfRequest();
            try
            {
                return jiraApiRequester.DoAuthenticatedRequest<User>(request);
            }
            catch (RequestDeniedException)
            {
                return new User();
            }
        }

        public string CreateIssue(int projectId, int issueTypeId, string summary, string description, string assignee)
        {
            var issue = new
            {
                fields = new 
                {
                    issuetype = new
                    {
                        id = issueTypeId
                    },
                    project = new
                    {
                        id = projectId
                    },
                    summary = summary,
                    description = description,
                    assignee = new
                    {
                        name = assignee
                    }
                }
            };

            var request = jiraApiRequestFactory.CreateCreateIssueRequest(issue);
            try
            {
                var createdIssue = jiraApiRequester.DoAuthenticatedRequest<Issue>(request);
                return createdIssue.Key;
            }
            catch (RequestDeniedException)
            {
                return null;
            }
        }


        public void AddAttachment(string issueKey, string filePath)
        {
            throw new NotImplementedException();
        }


        public void LinkIssues(string fromIssueKey, string toIssueKey)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region private members
        private IJiraApiRequestFactory jiraApiRequestFactory;
        private IJiraApiRequester jiraApiRequester;
        #endregion
    }
}
